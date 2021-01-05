namespace Discord.Bot.Utility
{
    /// <summary>
    /// Simple class for creating <see cref="Embed"/>s
    /// </summary>
    public static class Embedder
    {
        /// <summary>
        /// Creates a simple <see cref="Embed"/> with description set to <paramref name="text"/> and color set to either <paramref name="color"/> or <see cref="Color"/>(255, 255, 255)
        /// </summary>
        /// <param name="text">The description used for the embed</param>
        /// <param name="color">The color of the embed's side</param>
        /// <returns>Created <see cref="Embed"/</returns>
        public static Embed CreateSimple(string text, Color? color = null)
        {
            color ??= new Color(255, 255, 255);

            return NewBuilder().WithColor(color.GetValueOrDefault()).WithDescription(text).Build();
        }

        /// <summary>
        /// Creates a simple <see cref="Embed"/> with title set to <paramref name="title"/>, description set to <paramref name="text"/> and color set to either <paramref name="color"/> or <see cref="Color"/>(255, 255, 255)
        /// </summary>
        /// <param name="text">The description used for the embed</param>
        /// <param name="color">The color of the embed's side</param>
        /// <param name="title">The title to be used</param>
        /// <returns>Created <see cref="Embed"/></returns>
        public static Embed CreateSimple(string title, string text, Color? color = null)
        {
            color ??= new Color(255, 255, 255);

            return NewBuilder().WithColor(color.GetValueOrDefault()).WithDescription(text).WithTitle(title).Build();
        }

        /// <summary>
        /// Creates an embed using specified arguments
        /// </summary>
        /// <remarks>Because I was in a online class and I'm lazy, I am not going to make documentation for the arguments</remarks>
        /// <returns>Created <see cref="Embed"/</returns>
        public static Embed Create(string title = null, string text = null, IUser author = null, string thumbnailUrl = null, string imageUrl = null, Color? color = null)
        {
            var b = NewBuilder();
            
            if (title != null)
                b.WithTitle(title);

            if (text != null)
                b.WithDescription(text);

            if (color.HasValue)
                b.WithColor(color.Value);

            if (author != null)
                b.WithAuthor(author);

            if (thumbnailUrl != null)
                b.WithThumbnailUrl(thumbnailUrl);

            if (imageUrl != null)
                b.WithImageUrl(imageUrl);

            return b.Build();
        }

        private static EmbedBuilder NewBuilder()
        {
            return new EmbedBuilder();
        }
    }
}
