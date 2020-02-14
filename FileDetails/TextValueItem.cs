namespace FileDetails
{
    /// <summary>
    /// Represents a text value item which can be used for a combobox or something similar
    /// </summary>
    public sealed class TextValueItem
    {
        /// <summary>
        /// Gets the id of the entry
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the text
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Gets the value
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Creates a new instance of the <see cref="TextValueItem"/>
        /// </summary>
        /// <param name="text">The text</param>
        /// <param name="value">The value</param>
        public TextValueItem(string text, object value)
        {
            Text = text;
            Value = value;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TextValueItem"/>
        /// </summary>
        /// <param name="id">The id of the item</param>
        /// <param name="text">The text</param>
        /// <param name="value">The value</param>
        public TextValueItem(int id, string text, object value) : this(text, value)
        {
            Id = id;
        }

        /// <summary>
        /// Gets the text of the item
        /// </summary>
        /// <returns>The text</returns>
        public override string ToString()
        {
            return Text;
        }
    }
}
