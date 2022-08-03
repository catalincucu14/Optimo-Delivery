namespace ClientAdmin.Services.LocalStorage
{
    public interface ILocalStorageService
    {
        /// <summary>
        /// Function use to write the JWT in local storage
        /// </summary>
        public Task WriteTokenAsync(dynamic token);

        /// <summary>
        /// Function use to delete the JWT from local storage
        /// </summary>
        public Task RemoveTokenAsync();

        /// <summary>
        /// Function use to read the JWT from local storage
        /// </summary>
        public Task<string> ReadTokenAsync();
    }
}
