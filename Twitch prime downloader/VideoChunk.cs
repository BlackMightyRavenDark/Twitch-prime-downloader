
namespace Twitch_prime_downloader
{
    public sealed class VideoChunk
    {
        public long Position { get; private set; }
        public long Size { get; private set; }
        public string FileName { get; private set; }

        public VideoChunk(long position, long size, string fileName)
        {
            Position = position;
            Size = size;
            FileName = fileName;
        }
    }
}
