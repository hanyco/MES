namespace Library.CodeGenLib.Extensions;

public static class CodeExtension
{
    extension(Code)
    {
        /// <summary>
        /// Constructor for the <see cref="Code" /> with parameters.
        /// </summary>
        /// <param name="name">      Name of the code. </param>
        /// <param name="language">  Language of the code. </param>
        /// <param name="statement"> Statement of the code. </param>
        /// <param name="isPartial"> Whether the code is partial or not. </param>
        /// <param name="fileName">  File name of the code. </param>
        /// <returns> An instance of the <see cref="Code" />. </returns>
        public static Code Create((string Name, Language Language, string Statement, bool IsPartial) data) =>
            new(data.Name, data.Language, data.Statement, data.IsPartial);

        public static Code New(in string name, in Language language, in string statement, in bool isPartial = false, in string? fileName = null) =>
            new(name, language, statement, isPartial, fileName);

        public static Code NewEmpty() =>
                new(string.Empty, Languages.None, string.Empty);
    }

    extension(Code @this)
    {
        [return: NotNull]
        public Codes ToCodes() =>
            new(@this);

        [return: NotNull]
        public Code WithStatement([DisallowNull] string statement) =>
                new(@this) { Statement = statement };

        public void Deconstruct(out string name, out string statement) =>
                    (name, statement) = (@this.Name, @this.Statement);

        public void Deconstruct(out string name, out string statement, out bool isPartial) =>
            (name, statement, isPartial) = (@this.Name, @this.Statement, @this.IsPartial);

        public void Deconstruct(out string name, out Language language, out string statement, out bool isPartial) =>
            (name, language, statement, isPartial) = (@this.Name, @this.Language, @this.Statement, @this.IsPartial);

        public void Deconstruct(out string name, out Language language, out string statement, out bool isPartial, out string fileName) =>
            (name, language, statement, isPartial, fileName) = (@this.Name, @this.Language, @this.Statement, @this.IsPartial, @this.FileName);
    }

    extension(IEnumerable<Code> @this)
    {
        [return: NotNull]
        public Codes ToCodes() =>
            Codes.Create(@this);
    }

    public static bool IsNullOrEmpty([NotNullWhen(false)][AllowNull] this Code? @this) =>
        @this is null || @this.Equals(Code.Empty);
}

public static class CodesExtension
{
    extension(Codes)
    {
        /// <summary>
        /// Combines two Codes instances into one.
        /// </summary>
        /// <param name="x"> The first Codes instance. </param>
        /// <param name="y"> The second Codes instance. </param>
        /// <returns> A new Codes instance that combines the Code items from both input instances. </returns>
        public static Codes Combine(Codes x, Codes y) =>
            x + y;

        /// <summary>
        /// Creates a new instance of the Codes class.
        /// </summary>
        /// <returns> A new instance of the Codes class. </returns>
        public static Codes Create(params IEnumerable<Code> arg) =>
            new(arg);

        public static Codes Create(params IEnumerable<Codes> arg) =>
            new(arg.SelectAll());

        /// <summary>
        /// Creates a new instance of the Codes class that is empty.
        /// </summary>
        /// <returns> A new empty instance of the Codes class. </returns>
        public static Codes NewEmpty() =>
            new();
    }

    extension(Codes @this)
    {

    }

    extension(IEnumerable<Codes> @this)
    {
        public Codes GatherAll() =>
            Codes.Create(@this);

        [return: NotNull]
        public Codes ToCodes() =>
            Codes.Create(@this);
    }

    public static bool IsNullOrEmpty([NotNullWhen(false)][AllowNull] this Codes? @this) =>
        @this is null || @this.Count == 0 || @this.Equals(Codes.Empty);
}