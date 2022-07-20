using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace Tools
{
    public class Comms
    {
        private MemoryStream incomming = new MemoryStream();

        private bool DeSerialize(Byte[] serializedObject, out object deserialized, out string err)
        {
            deserialized    = null;
            err             = "";

            try {
                deserialized = new BinaryFormatter().Deserialize(new MemoryStream(serializedObject));
            }
            catch (Exception ex) {
                // This deserialize may fail if the calling assembly was a vb.net application.
                // Handling that is outside the scope of this exaple. However if you'd like 
                // to see that handled, have a look at my AbstractTcp project on Codeproject.com
                err = ex.Message;
                return false;
            }
            return true;
        }

        private static Byte[] Serialize(Object o)
        {
            Byte[] thisData                 = null;
            MemoryStream serializedData     = new MemoryStream();
            BinaryFormatter bf              = new BinaryFormatter();

            bf.Serialize(serializedData, o);

            if (serializedData.Length > 0) {
                serializedData.Position = 0;
                thisData                = serializedData.ToArray();
            }

            serializedData.SetLength(0);
            serializedData.Close();
            return thisData;
        }

        public static Byte[] CreatePacket(Object serializableObject)
        {
            Byte[] o                = null;
            Byte[] lengthBytes      = null;
            Byte[] packet           = null;
            Byte pType              = (byte)0;
                
            // Handle byte arrays differently (no serialization necessary):
            if (serializableObject.GetType() == typeof(Byte[])) {
                o               = (Byte[])serializableObject;
                lengthBytes     = BitConverter.GetBytes(o.Length);
            } else {
                o               = Serialize(serializableObject);
                lengthBytes     = BitConverter.GetBytes(o.Length);
                pType           = (byte)1;
            }

            // Create a new empty packet:
            packet = new Byte[o.Length + 5];
            // The first 4 bytes are the packet length. Write them:
            Array.Copy(lengthBytes, packet, 4);
            // The 5th byte is the packet type (byte array or serialized object):
            packet[4] = pType;
            // Lastly, write the data to the packet:
            Array.Copy(o, 0, packet, 5, o.Length);
            
            return packet;
        }

        public bool ReceivePacket(byte[] incommingBytes, int dataLength, out object deserializedObject)
        {
            bool complete       = false;
            int packetLength    = 0;
            deserializedObject  = null;
            byte[] buffer       = new byte[4];
            byte type           = 0;
            deserializedObject  = null;

            incomming.Write(incommingBytes, 0, dataLength);

            // Process Data:
            if(incomming.Length > 4) {
                // We have enough data to know how large the packet will be,
                // and what type it is:
                Array.Copy(incomming.GetBuffer(), 0, buffer, 0, 4);

                packetLength    = BitConverter.ToInt32(buffer, 0);
                type            = incomming.GetBuffer()[4];

                if (incomming.Length >= (packetLength+5))
                {
                    if(type == 0) {
                        // It's an array:
                        incomming.Position  = 5;
                        buffer              = new byte[packetLength];
                        incomming.Read(buffer, 0, packetLength);
                        deserializedObject  = buffer.Clone();
                    } else { 
                        // It's a serialized object:
                        incomming.Position  = 5;
                        buffer              = new byte[packetLength];
                        incomming.Read(buffer, 0, packetLength);

                        complete = DeSerialize(buffer, out deserializedObject, out string errMsg);
                        if (!complete) { 
                            // Handle deserialization failure here:
                        }
                    }

                    // Do we have more data in the incomming memorystream then just this packet's worth?
                    packetLength = (int)incomming.Length - (packetLength + 5);
                    if (packetLength > 0) {
                        buffer              = new byte[packetLength];
                        incomming.Read(buffer, 0, packetLength);
                        incomming.Position  = 0;
                        incomming.SetLength(0);
                        incomming.Write(buffer, 0, packetLength);
                    } else {
                        incomming.Position = 0;
                        incomming.SetLength(0);
                    }
                }
            }

            return complete;
        }
    }
}
