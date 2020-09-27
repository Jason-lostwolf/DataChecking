using System;

using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace DynamicDataChecking
{

    public class SoundHelper
    {
        private enum MsgBoxHelperFlags
        {
            SND_SYNC = 0x0000,  /* play synchronously (default) */
            SND_ASYNC = 0x0001,  /* play asynchronously */
            SND_NODEFAULT = 0x0002,  /* silence (!default) if sound not found */
            SND_MEMORY = 0x0004,  /* pszSound points to a memory file */
            SND_LOOP = 0x0008,  /* loop the sound until next sndPlaySound */
            SND_NOSTOP = 0x0010,  /* don't stop any currently playing sound */
            SND_NOWAIT = 0x00002000, /* don't wait if the driver is busy */
            SND_ALIAS = 0x00010000, /* name is a registry alias */
            SND_ALIAS_ID = 0x00110000, /* alias is a predefined ID */
            SND_FILENAME = 0x00020000, /* name is file name */
            SND_RESOURCE = 0x00040004  /* name is resource name or atom */
        }

        [DllImport("winmm.dll")]
        public static extern bool PlaySound(string pszSound,int hmod,int fdwSound);
        public const int SND_FILENAME = 0x00020000;
        public const int SND_ASYNC = 0x0001; 
 


        [DllImport("CoreDll.DLL", EntryPoint = "PlaySound", SetLastError = true)]
        private extern static int WCE_PlaySound(string szSound, IntPtr hMod, int flags);
        private static string GetSoundPath(SoundType st)
        {
            string str = "";
            switch (st)
            {
                case SoundType.Error:
                    str = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Sounds/报警提示.wav");
                    break;
                case SoundType.NoData:
                    str = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Sounds/NoData.wav");
                    //str = AppEnvironment.ApplicationPath + "Sounds\\NoData.wav";
                    break;
                case SoundType.SameData:
                    str = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Sounds/重复.mp3");
                    //str = AppEnvironment.ApplicationPath + "Sounds\\SameData.wav";
                    break;
                case SoundType.Succeed:
                    str = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Sounds/成功.mp3");
                    //str = AppEnvironment.ApplicationPath + "Sounds\\Succeed.wav";
                    break;
            }
            return str;

        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="st"></param>
        public static void PlaySound(SoundType st)
        {
            try
            {
                //MsgBoxHelper.WCE_PlaySound(MsgBoxHelper.GetSoundPath(st), IntPtr.Zero, (int)MsgBoxHelperFlags.SND_ASYNC);
                SoundHelper.PlaySound(SoundHelper.GetSoundPath(st), 0, SND_ASYNC | SND_FILENAME);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 播放声音
        /// </summary>
        /// <param name="st"></param>
        public static void PlaySound(string path)
        {
            try
            {
                //MsgBoxHelper.WCE_PlaySound(path, IntPtr.Zero, (int)MsgBoxHelperFlags.SND_ASYNC);
                SoundHelper.PlaySound(path, 0, SND_ASYNC | SND_FILENAME);
            }
            catch (Exception)
            {
            }
        }

    }
}
