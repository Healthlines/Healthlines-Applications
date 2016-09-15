using System;


namespace Questionnaires.Core.Services.Covertors
{
    public interface IConvertor<Tone, Ttwo>
        where Tone : class
        where Ttwo : class
    {
        Ttwo Convert(Tone source);
        Tone Convert(Ttwo source);

        void Fill(Tone source, Ttwo target);
        void Fill(Ttwo source, Tone target);
    }
}
