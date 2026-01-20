namespace PuzzleGames
{
    public static class StringUtils
    {
        public static string GetFormatTimeDefault() { return "{0}{1}{2}{3}"; }

        public static string DisplayTimeWithMinutes(this int timeToDisplay)
        {
            if (timeToDisplay < 0)
            {
                timeToDisplay = 0;
            }

            var formatTimer = GetFormatTimeDefault();

            var minutes = timeToDisplay % 60;
            var hours   = (int)(timeToDisplay / 60) % 24;
            var days    = (int)(timeToDisplay / (60 * 24));

            string GetFormatTime()
            {
                if (days > 0)
                {
                    return hours != 0
                        ? string.Format(formatTimer, $"{days}d ", $"{hours}h", string.Empty, string.Empty)
                        : string.Format(formatTimer, $"{days}d ", string.Empty, string.Empty, string.Empty);
                }

                if (hours > 0)
                {
                    return minutes != 0
                        ? string.Format(formatTimer, string.Empty, $"{hours}h ", $"{minutes}m", string.Empty)
                        : string.Format(formatTimer, string.Empty, $"{hours}h", string.Empty, string.Empty);
                }

                return string.Format(formatTimer, string.Empty, string.Empty, $"{minutes:00}", string.Empty);
            }

            return GetFormatTime();
        }
        
        public static string DisplayTimeWithSeconds(this int timeToDisplay)
        {
            if (timeToDisplay < 0)
            {
                timeToDisplay = 0;
            }

            var formatTimer = GetFormatTimeDefault();

            var seconds = (int)(timeToDisplay % 60);
            var minutes = (int)(timeToDisplay / 60) % 60;
            var hours   = (int)(timeToDisplay / 3600) % 24;
            var days    = (int)(timeToDisplay / (3600 * 24));

            string GetFormatTime()
            {
                if (days > 0)
                {
                    return hours != 0
                        ? string.Format(formatTimer, $"{days}d ", $"{hours}h", string.Empty, string.Empty)
                        : string.Format(formatTimer, $"{days}d ", string.Empty, string.Empty, string.Empty);
                }

                if (hours > 0)
                {
                    return minutes != 0
                        ? string.Format(formatTimer, string.Empty, $"{hours}h ", $"{minutes}m", string.Empty)
                        : string.Format(formatTimer, string.Empty, $"{hours}h", string.Empty, string.Empty);
                }

                return seconds != 0
                    ? string.Format(formatTimer, string.Empty, string.Empty, $"{minutes:00}:", $"{seconds:00}")
                    : string.Format(formatTimer, string.Empty, string.Empty, $"{minutes:00}", string.Empty);
            }

            return GetFormatTime();
        }
    }
}