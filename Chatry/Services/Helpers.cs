namespace Chatry.Services
{
    public static class Helpers
    {

        public static (string[] Strings, Enum_Results State)RoomDecoder(string RoomName)
        {
            if (string.IsNullOrWhiteSpace(RoomName) || !RoomName.Contains('-'))
            {
                return(null,Enum_Results.BREAK);
            }
            else
            {
                string[] IDs = RoomName.Split('-');
                return(IDs,Enum_Results.Silent);
            }
        }

        public static (int Int,Enum_Results State) ToInt(string String)
        {
            int Int = 0;
            if (Int32.TryParse(String, out  Int))
            {
                return(Int,Enum_Results.Silent);
            }
            else
            {
                return (Int,Enum_Results.BREAK);
            }
        }

        public static Enum_Results  IsEmpty(string String)
        {
            if (string.IsNullOrWhiteSpace(String))
            {
                return Enum_Results.BREAK;
            }
            else
            {
                return Enum_Results.Silent;
            }

        }









    }
}
