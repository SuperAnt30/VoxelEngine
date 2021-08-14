namespace VoxelEngine
{
    public delegate void StringEventHandler(object sender, StringEventArgs e);

    public class StringEventArgs
    {
        public StringEventArgs(string text)
        {
            Text = text;
        }

        public string Text { get; protected set; }
    }
}
