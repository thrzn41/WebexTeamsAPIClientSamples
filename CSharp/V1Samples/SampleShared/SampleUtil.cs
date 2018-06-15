/* 
 * MIT License
 * 
 * Copyright(c) 2018 thrzn41
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thrzn41.Util;
using Thrzn41.WebexTeams.Version1;

namespace SampleShared
{

    /// <summary>
    /// This util is used in the samples.
    /// </summary>
    public static class SampleUtil
    {
        public static ProtectedString LoadEncryptedToken()
        {
            var    userDir = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            string dataDir = String.Format("{0}{1}.thrzn41{1}WebexTeamsAPIClientSamples{1}V1Samples{1}", userDir.FullName, Path.DirectorySeparatorChar);

            byte[] token   = null;
            byte[] entropy = null;

            try
            {
                using (var stream = new FileStream(String.Format("{0}token.dat", dataDir), FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    token = memory.ToArray();
                }

                using (var stream = new FileStream(String.Format("{0}entropy.dat", dataDir), FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    entropy = memory.ToArray();
                }
            }
            catch (IOException) { }

            ProtectedString ps = null;

            if(token != null && entropy != null)
            {
                ps = LocalProtectedString.FromEncryptedData(token, entropy);

                // Tests decryption.
                try
                {
                    var data = ps.DecryptToChars();
                    ProtectedString.ClearChars(data);
                }
                catch(CryptographicException)
                {
                    ps = null;
                }
            }

            if(ps == null)
            {
                ShowMessage("Token is not found or valid. You must run 'S0010SetupSamples' to setup the samples first.");
            }

            return ps;
        }

        public static async Task<Space> FindSampleSpaceAsync(TeamsAPIClient teams)
        {
            Space space = null;

            var e = (await teams.ListSpacesAsync(type: SpaceType.Group)).GetListResultEnumerator();

            while(await e.MoveNextAsync())
            {
                var r = e.CurrentResult;

                if(r.IsSuccessStatus && r.Data.HasItems)
                {
                    foreach (var item in r.Data.Items)
                    {
                        if(item.Title.EndsWith("#WebexTeamsAPIClientV1SamplesSpace"))
                        {
                            space = item;
                            break;
                        }
                    }
                }
                
                if(space != null)
                {
                    // Sample space is found.
                    break;
                }
            }


            if(space == null)
            {
                ShowMessage("Sample space is not found. You must run 'S0010SetupSamples' to setup the samples first.");
            }

            return space;
        }

        /// <summary>
        /// Shows title and description for a sample.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="description">Description.</param>
        public static void ShowTitle(string title, string description)
        {
            Console.WriteLine("/*");
            Console.WriteLine(" * Title: {0}", title);
            Console.WriteLine(" * Description: {0}", description);
            Console.WriteLine(" */");
            Console.WriteLine();
        }


        /// <summary>
        /// Shows a message.
        /// </summary>
        /// <param name="message">message.</param>
        private static void showMessage(string message)
        {
            Console.WriteLine("[{0}]({1}) {2}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff"), Thread.CurrentThread.ManagedThreadId, message);
        }

        /// <summary>
        /// Shows a message.
        /// </summary>
        /// <param name="message">message.</param>
        /// <param name="args">args.</param>
        private static void showMessage(string message, params object[] args)
        {
            showMessage(String.Format(message, args));
        }

        /// <summary>
        /// Shows a message.
        /// </summary>
        /// <param name="message">message.</param>
        public static void ShowMessage(string message)
        {
            showMessage(message);
        }

        /// <summary>
        /// Shows a message.
        /// </summary>
        /// <param name="message">message.</param>
        /// <param name="args">args.</param>
        public static void ShowMessage(string message, params object[] args)
        {
            showMessage(message, args);
        }

        /// <summary>
        /// Shows an error.
        /// </summary>
        /// <param name="ex"><see cref="Exception"/> to be shown.</param>
        public static void ShowError(Exception ex)
        {
            showMessage(ex.ToString());
        }

        /// <summary>
        /// Waits key press.
        /// </summary>
        /// <param name="message">message.</param>
        /// <returns>Pressed key char.</returns>
        public static char WaitKeyPress(string message)
        {
            Console.WriteLine(message);

            return Console.ReadKey(true).KeyChar;
        }

        /// <summary>
        /// Waits key press.
        /// </summary>
        /// <param name="message">message.</param>
        /// <param name="key">Key to wait for pressing.</param>
        /// <returns>true if the specified key is pressed.</returns>
        public static bool WaitKeyPress(string message, char key)
        {
            Console.WriteLine(message);

            return (Console.ReadKey(true).KeyChar == key);
        }


        /// <summary>
        /// Waits key press to exit the app.
        /// </summary>
        public static void WaitKeyPressToExit()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
        }

    }

}
