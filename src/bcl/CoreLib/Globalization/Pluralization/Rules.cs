using System.Text.RegularExpressions;

namespace Library.Globalization.Pluralization;

internal static class Rules
{
    public static IEnumerable<(Regex Key, string Value)> PluralRules
    {
        get
        {
            yield return (new Regex("s?$", RegexOptions.IgnoreCase), "s");
            yield return (new Regex("[^\u0000-\u007F]$", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("([^aeiou]ese)$", RegexOptions.IgnoreCase), "$1");
            yield return (new Regex("(ax|test)is$", RegexOptions.IgnoreCase), "$1es");
            yield return (new Regex("(alias|[^aou]us|tlas|gas|ris)$", RegexOptions.IgnoreCase), "$1es");
            yield return (new Regex("(e[mn]u)s?$", RegexOptions.IgnoreCase), "$1s");
            yield return (new Regex("([^l]ias|[aeiou]las|[emjzr]as|[iu]am)$", RegexOptions.IgnoreCase), "$1");
            yield return (new Regex("(alumn|syllab|octop|vir|radi|nucle|fung|cact|stimul|termin|bacill|foc|uter|loc|strat)(?:us|i)$", RegexOptions.IgnoreCase), "$1i");
            yield return (new Regex("(alumn|alg|vertebr)(?:a|ae)$", RegexOptions.IgnoreCase), "$1ae");
            yield return (new Regex("(seraph|cherub)(?:im)?$", RegexOptions.IgnoreCase), "$1im");
            yield return (new Regex("(her|at|gr)o$", RegexOptions.IgnoreCase), "$1oes");
            yield return (new Regex("(agend|addend|millenni|dat|extrem|bacteri|desiderat|strat|candelabr|errat|ov|symposi|curricul|automat|quor)(?:a|um)$", RegexOptions.IgnoreCase), "$1a");
            yield return (new Regex("(apheli|hyperbat|periheli|asyndet|noumen|phenomen|criteri|organ|prolegomen|hedr|automat)(?:a|on)$", RegexOptions.IgnoreCase), "$1a");
            yield return (new Regex("sis$", RegexOptions.IgnoreCase), "ses");
            yield return (new Regex("(?:(kni|wi|li)fe|(ar|l|ea|eo|oa|hoo)f)$", RegexOptions.IgnoreCase), "$1$2ves");
            yield return (new Regex("([^aeiouy]|qu)y$", RegexOptions.IgnoreCase), "$1ies");
            yield return (new Regex("([^ch][ieo][ln])ey$", RegexOptions.IgnoreCase), "$1ies");
            yield return (new Regex("(x|ch|ss|sh|zz)$", RegexOptions.IgnoreCase), "$1es");
            yield return (new Regex("(matr|cod|mur|sil|vert|ind|append)(?:ix|ex)$", RegexOptions.IgnoreCase), "$1ices");
            yield return (new Regex("(m|l)(?:ice|ouse)$", RegexOptions.IgnoreCase), "$1ice");
            yield return (new Regex("(pe)(?:rson|ople)$", RegexOptions.IgnoreCase), "$1ople");
            yield return (new Regex("(child)(?:ren)?$", RegexOptions.IgnoreCase), "$1ren");
            yield return (new Regex("eaux$", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("m[ae]n$", RegexOptions.IgnoreCase), "men");
            yield return (new Regex("^thou$", RegexOptions.IgnoreCase), "you");
            yield return (new Regex("pox$", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("ois$", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("deer$", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("fish$", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("sheep$", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("measles$/", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("[^aeiou]ese$", RegexOptions.IgnoreCase), "$0");
        }
    }

    private static IEnumerable<(string Key, string Value)> Irregulars
    {
        get
        {
            // Pronouns.
            yield return ("I", "we");
            yield return ("me", "us");
            yield return ("he", "they");
            yield return ("she", "they");
            yield return ("them", "them");
            yield return ("myself", "ourselves");
            yield return ("yourself", "yourselves");
            yield return ("itself", "themselves");
            yield return ("herself", "themselves");
            yield return ("himself", "themselves");
            yield return ("themself", "themselves");
            yield return ("is", "are");
            yield return ("was", "were");
            yield return ("has", "have");
            yield return ("this", "these");
            yield return ("that", "those");
            // Words ending in with a consonant and `o`.
            yield return ("echo", "echoes");
            yield return ("dingo", "dingoes");
            yield return ("volcano", "volcanoes");
            yield return ("tornado", "tornadoes");
            yield return ("torpedo", "torpedoes");
            // Ends with `us`.
            yield return ("genus", "genera");
            yield return ("viscus", "viscera");
            // Ends with `ma`.
            yield return ("stigma", "stigmata");
            yield return ("stoma", "stomata");
            yield return ("dogma", "dogmata");
            yield return ("lemma", "lemmata");
            yield return ("schema", "schemata");
            yield return ("anathema", "anathemata");
            // Other irregular rules.
            yield return ("ox", "oxen");
            yield return ("axe", "axes");
            yield return ("die", "dice");
            yield return ("yes", "yeses");
            yield return ("foot", "feet");
            yield return ("eave", "eaves");
            yield return ("goose", "geese");
            yield return ("tooth", "teeth");
            yield return ("quiz", "quizzes");
            yield return ("human", "humans");
            yield return ("proof", "proofs");
            yield return ("carve", "carves");
            yield return ("valve", "valves");
            yield return ("looey", "looies");
            yield return ("thief", "thieves");
            yield return ("groove", "grooves");
            yield return ("pickaxe", "pickaxes");
            yield return ("whiskey", "whiskies");
        }
    }

    public static IEnumerable<(Regex Key, string Value)> SingularRules
    {
        get
        {
            yield return (new Regex("s$", RegexOptions.IgnoreCase), "");
            yield return (new Regex("(ss)$", RegexOptions.IgnoreCase), "$1");
            yield return (new Regex("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)(?:sis|ses)$", RegexOptions.IgnoreCase), "$1sis");
            yield return (new Regex("(^analy)(?:sis|ses)$", RegexOptions.IgnoreCase), "$1sis");
            yield return (new Regex("(wi|kni|(?:after|half|high|low|mid|non|night|[^\\w]|^)li)ves$", RegexOptions.IgnoreCase), "$1fe");
            yield return (new Regex("(ar|(?:wo|[ae])l|[eo][ao])ves$", RegexOptions.IgnoreCase), "$1f");
            yield return (new Regex("ies$", RegexOptions.IgnoreCase), "y");
            yield return (new Regex("\\b([pl]|zomb|(?:neck|cross)?t|coll|faer|food|gen|goon|group|lass|talk|goal|cut)ies$", RegexOptions.IgnoreCase), "$1ie");
            yield return (new Regex("\\b(mon|smil)ies$", RegexOptions.IgnoreCase), "$1ey");
            yield return (new Regex("(m|l)ice$", RegexOptions.IgnoreCase), "$1ouse");
            yield return (new Regex("(seraph|cherub)im$", RegexOptions.IgnoreCase), "$1");
            yield return (new Regex("(x|ch|ss|sh|zz|tto|go|cho|alias|[^aou]us|tlas|gas|(?:her|at|gr)o|ris)(?:es)?$", RegexOptions.IgnoreCase), "$1");
            yield return (new Regex("(e[mn]u)s?$", RegexOptions.IgnoreCase), "$1");
            yield return (new Regex("(movie|twelve)s$", RegexOptions.IgnoreCase), "$1");
            yield return (new Regex("(cris|test|diagnos)(?:is|es)$", RegexOptions.IgnoreCase), "$1is");
            yield return (new Regex("(alumn|syllab|octop|vir|radi|nucle|fung|cact|stimul|termin|bacill|foc|uter|loc|strat)(?:us|i)$", RegexOptions.IgnoreCase), "$1us");
            yield return (new Regex("(agend|addend|millenni|dat|extrem|bacteri|desiderat|strat|candelabr|errat|ov|symposi|curricul|quor)a$", RegexOptions.IgnoreCase), "$1um");
            yield return (new Regex("(apheli|hyperbat|periheli|asyndet|noumen|phenomen|criteri|organ|prolegomen|hedr|automat)a$", RegexOptions.IgnoreCase), "$1on");
            yield return (new Regex("(alumn|alg|vertebr)ae$", RegexOptions.IgnoreCase), "$1a");
            yield return (new Regex("(cod|mur|sil|vert|ind)ices$", RegexOptions.IgnoreCase), "$1ex");
            yield return (new Regex("(matr|append)ices$", RegexOptions.IgnoreCase), "$1ix");
            yield return (new Regex("(pe)(rson|ople)$", RegexOptions.IgnoreCase), "$1rson");
            yield return (new Regex("(child)ren$", RegexOptions.IgnoreCase), "$1");
            yield return (new Regex("(eau)x?$", RegexOptions.IgnoreCase), "$1");
            yield return (new Regex("men$", RegexOptions.IgnoreCase), "man");
            yield return (new Regex("pox$", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("ois$", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("deer$", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("fish$", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("sheep$", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("measles$/", RegexOptions.IgnoreCase), "$0");
            yield return (new Regex("[^aeiou]ese$", RegexOptions.IgnoreCase), "$0");
        }
    }

    public static IEnumerable<(string Key, string Value)> GetIrregularPlurals()
    {
        var result = new Dictionary<string, string>();
        foreach (var (Key, Value) in Irregulars.Reverse())
        {
            if (!result.ContainsKey(Value))
            {
                result.Add(Value, Key);
            }
        }
        return result.Select(kv => (kv.Key, kv.Value));
    }

    public static IEnumerable<(string Key, string Value)> GetIrregularSingulars()
        => Irregulars;

    public static IEnumerable<string> Uncountables
    {
        get
        {
            // Singular words with no plurals.
            yield return "advice";
            yield return "adulthood";
            yield return "agenda";
            yield return "aid";
            yield return "alcohol";
            yield return "ammo";
            yield return "athletics";
            yield return "bison";
            yield return "blood";
            yield return "bream";
            yield return "buffalo";
            yield return "butter";
            yield return "carp";
            yield return "cash";
            yield return "chassis";
            yield return "chess";
            yield return "clothing";
            yield return "commerce";
            yield return "cod";
            yield return "cooperation";
            yield return "corps";
            yield return "digestion";
            yield return "debris";
            yield return "diabetes";
            yield return "energy";
            yield return "equipment";
            yield return "elk";
            yield return "excretion";
            yield return "expertise";
            yield return "flounder";
            yield return "fun";
            yield return "gallows";
            yield return "garbage";
            yield return "graffiti";
            yield return "headquarters";
            yield return "health";
            yield return "herpes";
            yield return "highjinks";
            yield return "homework";
            yield return "housework";
            yield return "information";
            yield return "jeans";
            yield return "justice";
            yield return "kudos";
            yield return "labour";
            yield return "literature";
            yield return "machinery";
            yield return "mackerel";
            yield return "mail";
            yield return "media";
            yield return "mews";
            yield return "moose";
            yield return "music";
            yield return "news";
            yield return "pike";
            yield return "plankton";
            yield return "pliers";
            yield return "pollution";
            yield return "premises";
            yield return "rain";
            yield return "research";
            yield return "rice";
            yield return "salmon";
            yield return "scissors";
            yield return "series";
            yield return "sewage";
            yield return "shambles";
            yield return "shrimp";
            yield return "species";
            yield return "staff";
            yield return "swine";
            yield return "trout";
            yield return "traffic";
            yield return "transporation";
            yield return "tuna";
            yield return "wealth";
            yield return "welfare";
            yield return "whiting";
            yield return "wildebeest";
            yield return "wildlife";
            yield return "you";
        }
    }
}