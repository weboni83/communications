using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tools;

namespace AnonymousPipesClass
{
    public partial class frmMain : Form
    {
        private string[] theArgs;
        private AnonymousPipes pipe;
        private AnonymousPipes.Handles handles  = null;
        private bool closing                    = false;
        private string args                     = "";

        /// <summary>
        /// A handy little tool used to update the UI from a background thread.
        /// </summary>
        /// <param name="action">Code that runs on the UI thread.</param>
        private void UI(Action action)
        {
            if (closing) return;

            if (InvokeRequired) {
                try { Invoke((MethodInvoker)delegate () { action(); }); 
                } catch (Exception) { }
            } else {
                action();
            }
        }

        public frmMain(string[] args)
        {
            theArgs = args;
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            // Make sure when the application starts, none of the toolbar buttons are visible:
            tsbStopServer.Visible       = false;
            tsbStartServer.Visible      = false;
            tsbStartClient.Visible      = false;
            tsbConnectToPipe.Visible    = false;

            // In this example application, running it without the command line args will cause it to 
            // run as an AnonymousPipes Server, waiting for connections. 

            // Clicking the "Start Client" button will start a new instance of this application and pass the 
            // incomming and outgoing pipe handles as command line args. This will start a new example application
            // running as a client, hiding the "Start Client" button, and connecting to the server application using the pipe
            // handles in the command line args.
            if (theArgs.Length == 0) {
                // No command line args, so we're running as a server:
                // This form is cener screen. Client forms will be also. Let's do a little creative 
                // positioning: We'll shift this one off to the left by 1/2 of it's width. This way when
                // client forms appear and they shift themselves off to the right by 1/2 of their width, 
                // they will be side by side:
                Left = Left - (Width / 2);

                tsbStartServer.Visible  = true;
                tsbStartClient.Visible  = true;
                tsbStartClient.Enabled  = false;
                Text                    = "AnonymousPipes Server";
            }
            else {
                // We've been passed some command line args. We're a client!
                // Let's shift this form off to the right so they are both side by side:
                Left = Left + (Width / 2);

                // Set this application up as an AnonymousPipes client:
                args            = theArgs[0];
                Text            = "AnonymousPipes Client";

                tsbConnectToPipe.Visible = true;
            }
        }

        private void StartAnonymousPipesServer()
        {
            UI(() => tsbStatus.Text = "Waiting for a connection");
            UI(() => tsbStartClient.Enabled = true);

            // This is the AnonymousPipes helper class I put together to make this 
            // a little easier for myself (and you) to set up and use. I've set up a 
            // few simple delegates to handle the basic events: 
            //
            // 1.) callback: a delegate used to receive text from the pipe. In this example, 
            // I'm passing the text right off to the lbTextIn textbox in the UI.
            //
            // 2.) connectionEvent: This fires when a client connects. In this example, we are 
            // updating the form title, and disabling the "Start Client" button. 
            //
            // 3.) disconnectEvent: This fires when a client disconnects. In this example, we're 
            // adding an entry to lbTextIn which contains a disconnect notice, and the time of disconnection.
            // We are also closing the AnonymousPipes server, and starting it again so it can receive a new
            // connection.
            //
            // Lastly, we're receiving the handles of the Anonymous pipes which were created in this constructor
            // in the out handles object. We need pass these to the client when someone clicks the "Create Client"
            // button. 
            pipe = new AnonymousPipes("Server end of the pipe.", (object msg) => {
                // Text in:
                UI(() => lbTextIn.Items.Add("Object received: " + msg.GetType().ToString() + " (" + msg.ToString() + ")"));
            }, ()=> {
                // A client has connected!
                UI(() => tsbStatus.Text = "Client Connected");
                UI(() => tsbStartClient.Enabled = false);
            }, () => {
                // A client disconnected!
                UI(() => lbTextIn.Items.Add("Client disconnected at " + DateTime.Now.ToString()));

                // Shut down this AnonymousPipes server:
                pipe.Close();

                // And start it listening again
                // after the disconnection:
                if(!tsbStartServer.Visible) StartAnonymousPipesServer();
            }, out handles);

            UI(() => {
                tsbStartServer.Visible  = false;
                tsbStopServer.Visible   = true;
                lbTextIn.Items.Add("Current handles: " + handles.outGoing + ":::" + handles.inComing);
            });
        }

        /// <summary>
        /// When someone clicks the "Start Client" button, we need 
        /// to start a new application that connect to our
        /// AnonymousPipes server. This is where that happens.
        /// </summary>
        private void StartNewClientApplication()
        {
            try
            {
                // Start client application here:
                System.Diagnostics.Process pipeClient;
                
                pipeClient                              = new System.Diagnostics.Process();
                pipeClient.StartInfo.FileName           = Application.ExecutablePath;
                pipeClient.StartInfo.Arguments          = handles.outGoing + ":::" + handles.inComing;
                pipeClient.StartInfo.UseShellExecute    = false;
                pipeClient.Start();
            } catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Something went wrong starting the client!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// When this application starts as a client, it needs to connect to the AnonymousPipes 
        /// server. We do that using the pipe handles passed in the command line args. This is
        /// where that happens.
        /// </summary>
        /// <param name="startArgs"></param>
        private void ConnectToAnonymousPipesServer(string startArgs)
        {
            UI(() => Text = "Pipe Client");
            pipe          = new AnonymousPipes("Client end of the pipe.");

            if (!pipe.ConnectToPipe(startArgs, (object msg) => {
                UI(() => lbTextIn.Items.Add("Object received:" + msg.GetType().ToString() + " (" + msg.ToString() + ")"));
            }, () => {
                // We're disconnected!
                UI(() => tsbStatus.Text = "Disconnected");
                UI(() => tsbConnectToPipe.Enabled = false);
                pipe.Close();
            })) {
                // Connection failed!
                UI(() => tsbConnectToPipe.Enabled = false);
                UI(() => tsbStatus.Text = "Connection failed: The current handles are invalid!");
                return;
            }

            // We're connected!
            UI(() => tsbConnectToPipe.Enabled = false);
            UI(() => tsbStatus.Text = "Connected");
        }

        /// <summary>
        /// When the form closes, we shut down the AnonymousPipes helper class. Since we're using my UI helper
        /// function, there may be requests from background threads to update UI elements that have been disposed. 
        /// If those are allowed to execute, uncatchable exceptions will be thrown. We prevent that using the "closing"
        /// boolean. Once closing = true, the UI function will stop attempting to execute code on the UI thread, resolving
        /// this issue.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            closing = true;
            try { pipe.Close(); } catch (Exception) { }
        }

        private void tsbStartServer_Click(object sender, EventArgs e)
        {
            StartAnonymousPipesServer();
        }

        private void tsbStopServer_Click(object sender, EventArgs e)
        {
            tsbStopServer.Enabled   = false;
            tsbStatus.Text          = "Stopping pipe server:";

            ThreadPool.QueueUserWorkItem((o) => {
                pipe.Close();

                UI(() => {
                    tsbStopServer.Enabled   = true;
                    tsbStopServer.Visible   = false;
                    tsbStatus.Text          = "Pipe server stopped.";
                    tsbStartServer.Visible  = true;

                    handles = null;
                });
            });
        }

        private void btSendText_Click(object sender, EventArgs e)
        {
            if(pipe == null) {
                string msg = "";

                if(args.Equals("")) {
                    msg = "The AnonymousPipes server isn't running.";
                } else {
                    msg = "This client isn't connected to an AnonymousPipes server!";
                }

                MessageBox.Show(msg, "Oops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(textOut.Text.Trim().Length > 0) { 
                // We can drop any .net serializable object into the Send() function. If you'd like to 
                // send your own classes, make sure they exist in both your server and client applications,
                // that they are identical, and that you have marked them serializable.
                if(!pipe.Send(textOut.Text.Trim())) {
                    MessageBox.Show("Is there a client connected?", "There was a problem sending your message!", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tsbConnectToPipe_Click(object sender, EventArgs e)
        {
            ConnectToAnonymousPipesServer(args);
        }

        private void tsbStartClient_Click(object sender, EventArgs e)
        {
            if (handles == null) {
                MessageBox.Show("We can't start a client right now because the AnonymousPipe server hasn't been created yet, so the handles we need to pass to a client don't exist yet!",
                    "Oops! The AnonymousPipe server isn't running!",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            StartNewClientApplication();
        }
    }
}
