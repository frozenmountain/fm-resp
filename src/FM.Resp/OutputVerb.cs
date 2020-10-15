namespace FM.Resp
{
    abstract class OutputVerb<TOptions> : InputVerb<TOptions>
        where TOptions : OutputOptions
    {
        protected OutputVerb(TOptions options)
            : base(options)
        { }
    }
}
