using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;

namespace StoryShop.Speech
{
    public class SynthesizerSingleton
    {
        private static SpeechSynthesizer _speechSynthesizer;

        public static SpeechSynthesizer Instance
        {
            get
            {
                if (_speechSynthesizer == null)
                {
                    _speechSynthesizer = new SpeechSynthesizer();
                }

                return _speechSynthesizer;
            }
        }

     
    }
}
