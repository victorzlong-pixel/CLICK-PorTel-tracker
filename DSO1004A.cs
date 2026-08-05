using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace GS_Tracking_KR
{
    class DSO1004A
    {
        private static VisaInstrument myScope; 
        private bool initialized;

        public DSO1004A()
        {
            initialized = false;
            myScope = new VisaInstrument("USB0::0x0957::0x0588::CN50189034::0::INSTR");
        }

        public void initialize()
        {
            myScope.SetTimeoutSeconds(3);
            myScope.DoCommand("*CLS"); // clear status
            myScope.DoCommand("*RST"); // load default setup
            myScope.DoCommand(":DISPlay:MNUStatus 0");
            myScope.DoCommand(":CHANnel1:PROBe 10X"); 
            myScope.DoCommand(":CHANnel1:SCALe 1"); // sets 1 V/div
            myScope.DoCommand(":CHANnel1:OFFSet 0"); // sets V offset
            myScope.DoCommand(":TIMebase:MAIN:SCALe 0.00000001"); // sets screen time
            myScope.DoCommand(":TIMebase:MAIN:OFFSet 0.0");
            myScope.DoCommand(":TRIGger:SENSitivity 3");
            myScope.DoCommand(":ACQuire:TYPE NORMal");
            myScope.DoCommand(":RUN");
            Thread.Sleep(5000);
            initialized = true;
        }

        public double getVavg()
        {
            if (!initialized)
                return -1;
            double Vavg;
            //myScope.DoCommand(":RUN"); // capture single acquisition
            myScope.DoCommand(":MEASure:SOURce CHANnel1");
            Vavg = myScope.DoQueryNumber(":MEASure:VAVerage?");
            //myScope.DoCommand(":RUN");
            return Vavg;
        }

        public void close()
        {
            myScope.Unlock();
            myScope.Close();
        }
    }

    class VisaInstrument
    {
        private int m_nResourceManager;
        private int m_nSession;
        private string m_strVisaAddress;
        // Constructor.
        public VisaInstrument(string strVisaAddress)
        {
            // Save VISA addres in member variable.
            m_strVisaAddress = strVisaAddress;
            // Open the default VISA resource manager.
            OpenResourceManager();
            // Open a VISA resource session.
            OpenSession();
            // Clear the interface.
            int nViStatus;
            nViStatus = visa32.viClear(m_nSession);
        }
        public void DoCommand(string strCommand)
        {
            // Send the command.
            VisaSendCommandOrQuery(strCommand);
            // Wait for operation complete and check for inst errors.
            WaitOperationComplete();
            CheckInstrumentErrors(strCommand);
        }
        public int DoCommandIEEEBlock(string strCommand,
            byte[] DataArray)
        {
            // Send the command to the device.
            string strCommandAndLength;
            int nViStatus, nLength, nBytesWritten;
            nLength = DataArray.Length;
            strCommandAndLength = String.Format("{0} #8%08d",
                strCommand);
            // Write first part of command to formatted I/O write buffer.
            nViStatus = visa32.viPrintf(m_nSession, strCommandAndLength,
                nLength);
            CheckVisaStatus(nViStatus);
            // Write the data to the formatted I/O write buffer.
            nViStatus = visa32.viBufWrite(m_nSession, DataArray, nLength,
                out nBytesWritten);
            CheckVisaStatus(nViStatus);
            // Wait for operation complete and check for inst errors.
            WaitOperationComplete();
            CheckInstrumentErrors(strCommand);
            return nBytesWritten;
        }
        public StringBuilder DoQueryString(string strQuery)
        {
            // Send the query.
            VisaSendCommandOrQuery(strQuery);
            // Get the result string.
            StringBuilder strResults = new StringBuilder(1000);
            strResults = VisaGetResultString();
            // Wait for operation complete and check for inst errors.
            WaitOperationComplete();
            CheckInstrumentErrors(strQuery);
            // Return string results.
            return strResults;
        }
        public double DoQueryNumber(string strQuery)
        {
            // Send the query.
            VisaSendCommandOrQuery(strQuery);
            // Get the result string.
            double fResults;
            fResults = VisaGetResultNumber();
            // Wait for operation complete and check for inst errors.
            WaitOperationComplete();
            CheckInstrumentErrors(strQuery);
            // Return string results.
            return fResults;
        }
        public double[] DoQueryNumbers(string strQuery)
        {
            // Send the query.
            VisaSendCommandOrQuery(strQuery);
            // Get the result string.
            double[] fResultsArray;
            fResultsArray = VisaGetResultNumbers();
            // Wait for operation complete and check for inst errors.
            WaitOperationComplete();
            CheckInstrumentErrors(strQuery);
            // Return string results.
            return fResultsArray;
        }
        public int DoQueryIEEEBlock(string strQuery,
            out byte[] ResultsArray)
        {
            // Send the query.
            VisaSendCommandOrQuery(strQuery);
            // Get the result string.
            System.Threading.Thread.Sleep(2000); // Delay before reading.
            int length; // Number of bytes returned from instrument.
            length = VisaGetResultIEEEBlock(out ResultsArray);
            // Wait for operation complete and check for inst errors.
            WaitOperationComplete();
            CheckInstrumentErrors(strQuery);
            // Return string results.
            return length;
        }
        public void Unlock()
        {
            VisaSendCommandOrQuery(":KEY:LOCK DISable");
        }
        private void VisaSendCommandOrQuery(string strCommandOrQuery)
        {
            // Send command or query to the device.
            string strWithNewline;
            strWithNewline = String.Format("{0}\n", strCommandOrQuery);
            int nViStatus;
            nViStatus = visa32.viPrintf(m_nSession, strWithNewline);
            CheckVisaStatus(nViStatus);
        }
        private StringBuilder VisaGetResultString()
        {
            StringBuilder strResults = new StringBuilder(1000);
            // Read return value string from the device.
            int nViStatus;
            nViStatus = visa32.viScanf(m_nSession, "%1000t", strResults);
            CheckVisaStatus(nViStatus);
            return strResults;
        }
        private double VisaGetResultNumber()
        {
            double fResults = 0;
            // Read return value string from the device.
            int nViStatus;
            nViStatus = visa32.viScanf(m_nSession, "%lf", out fResults);
            CheckVisaStatus(nViStatus);
            return fResults;
        }
        private double[] VisaGetResultNumbers()
        {
            double[] fResultsArray;
            fResultsArray = new double[10];
            // Read return value string from the device.
            int nViStatus;
            nViStatus = visa32.viScanf(m_nSession, "%,10lf\n",
                fResultsArray);
            CheckVisaStatus(nViStatus);
            return fResultsArray;
        }
        private int VisaGetResultIEEEBlock(out byte[] ResultsArray)
        {
            // Results array, big enough to hold a PNG.
            ResultsArray = new byte[300000];
            int length; // Number of bytes returned from instrument.
                        // Set the default number of bytes that will be contained in
                        // the ResultsArray to 300,000 (300kB).
            length = 300000;
            // Read return value string from the device.
            int nViStatus;
            nViStatus = visa32.viScanf(m_nSession, "%#b", ref length,
                ResultsArray);
            CheckVisaStatus(nViStatus);
            // Write and read buffers need to be flushed after IEEE block?
            nViStatus = visa32.viFlush(m_nSession, visa32.VI_WRITE_BUF);
            CheckVisaStatus(nViStatus);
            nViStatus = visa32.viFlush(m_nSession, visa32.VI_READ_BUF);
            CheckVisaStatus(nViStatus);
            return length;
        }
        private void WaitOperationComplete()
        {
            // Wait for operation to complete.
            StringBuilder strOpcResult = new StringBuilder(1000);
            do
            {
                // Small wait to prevent excessive queries.
                System.Threading.Thread.Sleep(100);
                VisaSendCommandOrQuery("*OPC?");
                strOpcResult = VisaGetResultString();
            } while (!strOpcResult.ToString().StartsWith("1"));
        }
        private void CheckInstrumentErrors(string strCommand)
        {
            // Check for instrument errors.
            StringBuilder strInstrumentError = new StringBuilder(1000);
            bool bFirstError = true;
            do // While not "0,No error"
            {
                VisaSendCommandOrQuery(":SYSTem:ERRor?");
                strInstrumentError = VisaGetResultString();
                if (!strInstrumentError.ToString().StartsWith("0,"))
                {
                    if (bFirstError)
                    {
                        Console.WriteLine("ERROR(s) for command '{0}': ",
                            strCommand);
                        bFirstError = false;
                    }
                    Console.Write(strInstrumentError);
                }
            } while (!strInstrumentError.ToString().StartsWith("0,"));
        }
        private void OpenResourceManager()
        {
            int nViStatus;
            nViStatus =
                visa32.viOpenDefaultRM(out this.m_nResourceManager);
            if (nViStatus < visa32.VI_SUCCESS)
                throw new
                ApplicationException("Failed to open Resource Manager");
        }
        private void OpenSession()
        {
            int nViStatus;
            nViStatus = visa32.viOpen(this.m_nResourceManager,
                this.m_strVisaAddress, visa32.VI_NO_LOCK,
                visa32.VI_TMO_IMMEDIATE, out this.m_nSession);
            CheckVisaStatus(nViStatus);
        }
        public void SetTimeoutSeconds(int nSeconds)
        {
            int nViStatus;
            nViStatus = visa32.viSetAttribute(this.m_nSession,
                visa32.VI_ATTR_TMO_VALUE, nSeconds * 1000);
            CheckVisaStatus(nViStatus);
        }
        public void CheckVisaStatus(int nViStatus)
        {
            // If VISA error, throw exception.
            if (nViStatus < visa32.VI_SUCCESS)
            {
                StringBuilder strError = new StringBuilder(256);
                visa32.viStatusDesc(this.m_nResourceManager, nViStatus,
                    strError);
                throw new ApplicationException(strError.ToString());
            }
        }
        public void Close()
        {
            if (m_nSession != 0)
                visa32.viClose(m_nSession);
            if (m_nResourceManager != 0)
                visa32.viClose(m_nResourceManager);
        }
    }
}
