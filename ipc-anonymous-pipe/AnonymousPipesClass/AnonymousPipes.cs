using System;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Tools;

namespace AnonymousPipesClass
{
    public class AnonymousPipes
    {
        private AnonymousPipeServerStream outGoingServerPipe;
        private AnonymousPipeServerStream inComingServerPipe;
        private PipeStream clientIn;
        private PipeStream clientOut;
        private string incomingHandle;
        private string outgoingHandle;
        private StreamWriter ssw;
        private StreamWriter csw;
        private bool serverMode;
        private bool running;
        private CallBack callback;
        private ConnectionEvent disconnectEvent;
        private ConnectionEvent connectionEvent;
        private string msgError;
        private string name;
        private StreamReader isr;
        private bool disconnecting      = false;
        private bool clientSyncRecieved = false;
        private bool connectionActive   = false;
        private bool closing            = false;
        private Comms comm              = new Comms();

        public delegate void CallBack(object msg);
        public delegate void ConnectionEvent();
        public string ermsg;

        // Threading / Task simplifiers:
        private Task tsk(Action code) {
            return Task.Factory.StartNew(code, TaskCreationOptions.PreferFairness);
        }
        private Task longTsk(Action code) {
            return Task.Factory.StartNew(code, TaskCreationOptions.LongRunning);
        }

        public bool ConnectionActive()
        {
            return connectionActive;
        }

        public string GetPipeName()
        {
            return name;
        }

        public AnonymousPipes(String pipeName)
        {
            this.name = pipeName;
        }

        public class Handles
        {
            public string outGoing;
            public string inComing;

            public Handles(string outGoing, string inComing) {
                this.outGoing = outGoing;
                this.inComing = inComing;
            }
        }

        /// <summary>
        /// Here we initialize the two communication pipes, and
        /// return the handles. 
        /// </summary>
        /// <returns>A Handles object, contaning the handles for the pipes.</returns>
        private Handles InitializePipes()
        {
            serverMode = true;
            outGoingServerPipe = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
            inComingServerPipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);

            return new Handles(outGoingServerPipe.GetClientHandleAsString(), inComingServerPipe.GetClientHandleAsString());
        }

        private void StartDisconnectionMonitor()
        {
            longTsk(() => {

                // Watch for disconnections:
                while (!disconnecting) Thread.Sleep(10);

                // If Close() has been called, nobody is interested in
                // receiving a disconnect event. Just bail:
                if (closing) return;

                // Close the incomming message stream (it will block unles we do this):
                try { ssw.Close(); } catch (Exception) { }

                // Report that we have a disconnection:
                disconnectEvent();
                connectionActive = false;
            });
        }

        public AnonymousPipes(String pipeName, CallBack callback, ConnectionEvent connectionEvent, ConnectionEvent disconnectEvent, out Handles handles)
        {
            this.callback                   = callback;
            this.disconnectEvent            = disconnectEvent;
            this.connectionEvent            = connectionEvent;
            this.name                       = pipeName;
            this.running                    = true;
            serverMode                      = true;
            byte[] buffer                   = new byte[32768];
            int received                    = 0;
            handles                         = InitializePipes();
            Comms comm                      = new Comms();
            object dataIn                   = null;

            longTsk(() => {
                while(running) {
                    ssw                     = new StreamWriter(outGoingServerPipe);
                    ssw.AutoFlush           = true;

                    buffer = Comms.CreatePacket("SYNC");
                    ssw.BaseStream.Write(buffer, 0, buffer.Length);

                    // Wait for a connection:
                    try { outGoingServerPipe.WaitForPipeDrain(); } catch (Exception) { }

                    if (!running) return;

                    while (!inComingServerPipe.IsConnected && running)
                        Thread.Sleep(10);

                    // Let the user know we have a connection!
                    connectionActive = true;
                    connectionEvent();

                    // Watch for disconnections:
                    StartDisconnectionMonitor();

                    // Accept incomming messages:
                    buffer = new byte[32768];
                    using (isr = new StreamReader(inComingServerPipe))
                    {
                        while (running && inComingServerPipe.IsConnected)
                        {
                            received = isr.BaseStream.Read(buffer, 0, 32768);

                            if(comm.ReceivePacket(buffer, received, out dataIn)) { 
                                if(dataIn.GetType().Equals(typeof(string)) && !clientSyncRecieved)
                                {
                                    if(((string)dataIn).StartsWith("SYNC") ) {
                                        clientSyncRecieved = true;
                                    } 
                                } else {
                                    if (dataIn.GetType().Equals(typeof(string)) && ((string)dataIn).Equals("<::disconnecting::>"))
                                    {
                                        disconnecting = true;
                                    } else { callback(dataIn); }
                                }
                            }
                        }
                    }
                }
                
                running = false;
            });
        }

        public bool Send(object msg)
        {
            return SendObject(msg, ref msgError);
        }

        public bool SendObject(object msg, ref String errMsg)
        {
            byte[] buffer;

            if (serverMode) {
                if(!connectionActive) {
                    errMsg = "No connected client.";
                    return false;
                }

                try {
                    buffer = Comms.CreatePacket(msg);
                    ssw.BaseStream.Write(buffer, 0, buffer.Length);
                    outGoingServerPipe.WaitForPipeDrain();

                    return true;
                } catch (Exception ex) {
                    errMsg = ex.Message;
                    return false;
                }
            } else {
                if(!connectionActive) {
                    errMsg = "Not connected.";
                    return false;
                }

                try {
                    buffer = Comms.CreatePacket(msg);
                    csw.BaseStream.Write(buffer, 0, buffer.Length);
                    clientOut.WaitForPipeDrain();
                }
                catch (Exception) { }
                return true;
            }
        }

        public bool ConnectToPipe(String clientHandles, CallBack callback, ConnectionEvent disconnectEvent)
        {
            string[] handles        = System.Text.RegularExpressions.Regex.Split(clientHandles, ":::");
            StreamReader sr         = null;
            bool syncReceived       = false;
            this.incomingHandle     = handles[0];
            this.outgoingHandle     = handles[1];
            this.callback           = callback;
            this.disconnectEvent    = disconnectEvent;
            running                 = true;
            disconnecting           = false;
            serverMode              = false;
            object dataIn;
            byte[] buffer           = new byte[32768];
            int received            = 0;

            // Initialize our connection, and send (and receive) an initial "SYNC" 
            // message to verify successful communication:
            try {
                clientIn        = new AnonymousPipeClientStream(PipeDirection.In, this.incomingHandle);
                clientOut       = new AnonymousPipeClientStream(PipeDirection.Out, this.outgoingHandle);

                csw             = new StreamWriter(clientOut);
                csw.AutoFlush   = true;

                sr              = new StreamReader(clientIn);

                while(!syncReceived) {
                    received = sr.BaseStream.Read(buffer, 0, 32768);

                    if(comm.ReceivePacket(buffer, received, out dataIn)) { 
                        if(dataIn.GetType().Equals(typeof(string)))
                        {
                            if(((string)dataIn).StartsWith("SYNC")) {
                                syncReceived = true;
                            }
                        }
                    }
                }

                try {
                    buffer = Comms.CreatePacket("SYNC");
                    csw.BaseStream.Write(buffer, 0, buffer.Length);
                    clientOut.WaitForPipeDrain(); 
                } catch (Exception) {
                    connectionActive    = false;
                    return false;
                }

                connectionActive    = true;
            } catch (Exception) {
                // The pipes are invaid.
                return false;
            }

            // Start the incomming message loop on a long running
            // background thread, and allow this function to return:
            longTsk(() => { 
                
                buffer = new byte[32768];
                while (running && clientIn.IsConnected) {
                    received = sr.BaseStream.Read(buffer, 0, 32768);

                    if(comm.ReceivePacket(buffer, received, out dataIn)) { 
                        if(dataIn.GetType().Equals(typeof(string)))
                        {
                            if(((string)dataIn).Equals("<::disconnecting::>")) {
                                disconnecting = true;
                            } else { callback(dataIn); }
                        } else {
                            callback(dataIn);
                        }
                    }
                }

                running             = false;
                connectionActive    = false;
                disconnectEvent();

                try { sr.Close();
                } catch (Exception) { }
            });

            return true;
        }
        public void Close()
        {
            // Send a disconnect notice on a background thread,
            // waiting for it to complete or timing out after 1 second and moving on:
            tsk(() => Send("<::disconnecting::>")).Wait(1000);

            closing             = true;
            disconnecting       = true;
            running             = false;
            connectionActive    = false;

            try{ outGoingServerPipe.Close();
            } catch (Exception) { }

            try { inComingServerPipe.Close();
            } catch (Exception) { }

            try { clientOut.Close();
            } catch (Exception) { }

            try { clientIn.Close();
            } catch (Exception) { }

            try { ssw.Close();
            } catch (Exception) { }

            try { csw.Close();
            } catch (Exception) { }

            try {
                outGoingServerPipe.DisposeLocalCopyOfClientHandle();
                inComingServerPipe.DisposeLocalCopyOfClientHandle();
            } catch (Exception) { }

            try { isr.Close();
            } catch (Exception) { }
        }
    }
}
