namespace CargoTrack.Business.Helpers
{
    public static class PrivacyMasker
    {
        private const char MaskChar = '*';
        private const int MaskLength = 3;

        public static string MaskFullName(string firstName, string lastName)
        {
            return $"{MaskWord(firstName)} {MaskWord(lastName)}";
        }

        private static string MaskWord(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new string(MaskChar, MaskLength);
            }

            return value.Trim()[0] + new string(MaskChar, MaskLength);
        }
    }
}
