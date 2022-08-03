#nullable disable

namespace Models
{
    public class Response<T>
    {
        public bool Success { get; set; }

        public T Data { get; set; }

        public List<string> Errors { get; set; }
    }
}
