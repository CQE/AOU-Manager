using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace DemoPrototype
{

    public class StreamCRC32
    {
        const UInt32 CRC_INIT = 0x0001;
        const UInt32 CRC_ADD_BYTES = 0x0002;
        const UInt32 CRC_VALUE = 0x0008;
        private static UInt32 crc = 0xFFFFFFFFU;

        /*  This is the basic CRC-32 calculation with some optimization but no
            table lookup. The the byte reversal is avoided by shifting the crc reg
            right instead of left and by using a reversed 32-bit word to represent
            the polynomial.

            When compiled to Cyclops with GCC, this function executes in 8 + 72n
            instructions, where n is the number of bytes in the input message.
            It should be doable in 4 + 61n instructions.
            If the inner loop is strung out (approx. 5*8 = 40 instructions),
            it would take about 6 + 46n instructions.

              http://www.hackersdelight.org/hdcodetxt/crc.c.txt

            Check values etc (asci data "123456789" with CRC32 should result in CRC 0xCBF43926):
              http://www.barrgroup.com/Embedded-Systems/How-To/CRC-Calculation-C-Code
        */

        static UInt32 crc32b(byte[] message, int nbytes, uint mode)
        {
            int i, j;
            UInt32 mask;

            // Reset CRC?
            if ((mode & CRC_INIT) != 0)
                crc = 0xFFFFFFFFU;

            // Add bytes to CRC?
            if ((mode & CRC_ADD_BYTES) != 0 && nbytes > 0 && message != null)
            {

                for (i = 0; i < nbytes; i++)
                {
                    // Get next byte
                    crc = crc ^ ((UInt32)message[i]);

                    // Do eight times
                    for (j = 7; j >= 0; j--)
                    {
                        mask = (UInt32)(((crc & 1) != 0) ? -1 : 0);    // Provides 0 or -1 depending on the least significant bit
                        crc = (crc >> 1) ^ (0xEDB88320U & mask);
                    }
                }

            }

            // Return CRC value
            return (~crc);
        }

        /*
            byte[] buffer = Encoding.ASCII.GetBytes("123456789");  // Its proper CRC32 value is 0xCBF43926
            UInt32 ulVal;
            String junk;

            // As one buffer
            crc32b(null, 0, CRC_INIT);
            crc32b(buffer, 9, CRC_ADD_BYTES);
             ulVal = crc32b(null, 0, CRC_VALUE);

             Console.WriteLine("CRC32 value == 0x{0:X}", ulVal);


            // As multiple buffers
            crc32b(null, 0, CRC_INIT);
            crc32b(buffer.Skip(0).Take(5).ToArray(), 5, CRC_ADD_BYTES);
            crc32b(buffer.Skip(5).Take(4).ToArray(), 4, CRC_ADD_BYTES);
            ulVal = crc32b(null, 0, CRC_VALUE);

            Console.WriteLine("CRC32 value == 0x{0:X}", ulVal);
 
 
            // Pause waiting for Console input
            junk = Console.ReadLine();
       */
    }


    [StructLayout(LayoutKind.Sequential, Pack = 2)] // Pack = 0, 1, 2, 4, 8, 16, 32, 64, or 128:
    public struct AOUStateData
    {
        public UInt16 time_hours;             // Max 64000 hours > 7 years
        public UInt16 time_sek_x_10_of_hour; // 60 sek * 10 * 60 min = 36000 (sek x 10)

        public UInt16 coldTankTemp;
        public UInt16 hotTankTemp;
        public UInt16 retTemp;

        public Int16 RetForTemp;

        public UInt16 coolerTemp;
        public UInt16 coolantTemp;
        public UInt16 heaterTemp;

        public UInt16 bufHotTemp;
        public UInt16 bufMidTemp;
        public UInt16 bufColdTemp;

        public UInt16 seqState;

        public UInt16 BearHot;

        public UInt16 Power;

        public UInt16 Energy;
        public UInt16 Valves;
        public UInt16 Safety;
        public UInt16 IMM;
        public UInt16 UIButtons;
        public Int16 Mode;
    }
}
