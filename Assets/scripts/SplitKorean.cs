using System.Text;

public static class HangulUtils
{
    // 초성, 중성, 종성 매핑
    private static readonly string[] Initials = {
        "ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ", "ㅅ", "ㅆ",
        "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"
    };

    private static readonly string[] Vowels = {
        "ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅘ", "ㅙ",
        "ㅚ", "ㅛ", "ㅜ", "ㅝ", "ㅞ", "ㅟ", "ㅠ", "ㅡ", "ㅢ", "ㅣ"
    };

    private static readonly string[] Finals = {
        "", "ㄱ", "ㄲ", "ㄳ", "ㄴ", "ㄵ", "ㄶ", "ㄷ", "ㄹ", "ㄺ", "ㄻ", "ㄼ",
        "ㄽ", "ㄾ", "ㄿ", "ㅀ", "ㅁ", "ㅂ", "ㅄ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅊ",
        "ㅋ", "ㅌ", "ㅍ", "ㅎ"
    };

    // 겹자음 분리
    private static readonly string[][] CompoundConsonants = {
        new[] { "ㄱ", "ㄱ" }, new[] { "ㄱ", "ㅅ" }, new[] { "ㄴ", "ㅈ" }, new[] { "ㄴ", "ㅎ" },
        new[] { "ㄹ", "ㄱ" }, new[] { "ㄹ", "ㅁ" }, new[] { "ㄹ", "ㅂ" }, new[] { "ㄹ", "ㅅ" },
        new[] { "ㄹ", "ㅌ" }, new[] { "ㄹ", "ㅍ" }, new[] { "ㄹ", "ㅎ" }, new[] { "ㅂ", "ㅅ" }
    };

    // 겹모음 분리
    private static readonly string[][] CompoundVowels = {
        new[] { "ㅗ", "ㅏ" }, new[] { "ㅗ", "ㅐ" }, new[] { "ㅗ", "ㅣ" },
        new[] { "ㅜ", "ㅓ" }, new[] { "ㅜ", "ㅔ" }, new[] { "ㅜ", "ㅣ" },
        new[] { "ㅡ", "ㅣ" }
    };

    public static string Decompose(string input)
    {
        StringBuilder decomposed = new StringBuilder();

        foreach (char c in input)
        {
            if (c >= 0xAC00 && c <= 0xD7A3) // 한글 여부 확인
            {
                int unicode = c - 0xAC00;

                int initialIndex = unicode / (21 * 28); // 초성
                int vowelIndex = (unicode % (21 * 28)) / 28; // 중성
                int finalIndex = unicode % 28; // 종성

                // 초성 분리
                decomposed.Append(SplitCompound(Initials[initialIndex]));

                // 중성 분리
                decomposed.Append(SplitCompound(Vowels[vowelIndex]));

                // 종성 분리
                if (finalIndex > 0)
                    decomposed.Append(SplitCompound(Finals[finalIndex]));
            }
            else
            {
                decomposed.Append(c); // 한글이 아닌 문자는 그대로 추가
            }
        }

        return decomposed.ToString();
    }

    // 겹자음/겹모음 분리 함수
    private static string SplitCompound(string character)
    {
        foreach (var compound in CompoundConsonants)
        {
            if (character == string.Concat(compound))
            {
                return string.Join(" ", compound);
            }
        }

        foreach (var compound in CompoundVowels)
        {
            if (character == string.Concat(compound))
            {
                return string.Join(" ", compound);
            }
        }

        return character; // 단일 자음/모음은 그대로 반환
    }
}
