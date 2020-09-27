using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicDataChecking
{
    public class ScanSoundHelper
    {
        public static void PlaySound(SoundType st)
        {            
            clsMCI c = new clsMCI();
            c.FileName = GetSoundFilePath(st);
            c.play();
        }

        private static string GetSoundFilePath(SoundType st)
        {
            string soundPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Sounds");
            string soundFilePath = string.Empty;
            switch(st)
            {
                case SoundType.Error:
                    soundFilePath = System.IO.Path.Combine(soundPath, "Error.wav");
                    break;
                case SoundType.BarcodeNotSuccess:
                    soundFilePath = System.IO.Path.Combine(soundPath, "BarcodeNotSuccess.mp3");
                    break;
                case SoundType.BarcodeSuccessScanColorBox:
                    soundFilePath = System.IO.Path.Combine(soundPath, "BarcodeSuccessScanColorBox.mp3");
                    break;
                case SoundType.FinishOneInnerBox:
                    soundFilePath = System.IO.Path.Combine(soundPath, "FinishOneInnerBox.mp3");
                    break;
                case SoundType.OneOutsiteBoxFinish:
                    soundFilePath = System.IO.Path.Combine(soundPath, "OneOutsiteBoxFinish.mp3");
                    break;
                case SoundType.SameData:
                    soundFilePath = System.IO.Path.Combine(soundPath, "SameData.mp3");
                    break;
                case SoundType.ScanFirstInnerBox:
                    soundFilePath = System.IO.Path.Combine(soundPath, "ScanFirstInnerBox.mp3");
                    break;
                case SoundType.Succeed:
                    soundFilePath = System.IO.Path.Combine(soundPath, "Succeed.mp3");
                    break;
                case SoundType.OneBoxNotFinish:
                    soundFilePath = System.IO.Path.Combine(soundPath, "OneBoxNotFinish.wav");
                    break;
            }

            return soundFilePath;
        }


    }
}
