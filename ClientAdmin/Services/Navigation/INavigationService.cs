namespace ClientAdmin.Services.Navigation
{
    public interface INavigationService
    {
        /// <summary>
        /// Function used to redirect to /
        /// </summary>
        public void ToHome();

        /// <summary>
        /// Function used to redirect to /server-error
        /// </summary>
        public void ToServerError();

        /// <summary>
        /// Function used to redirect to /help
        /// </summary>
        public void ToHelp();

        /// <summary>
        /// Function used to redirect to /authentication/login
        /// </summary>
        public void ToLogin();

        /// <summary>
        /// Function used to redirect to /authentication/register
        /// </summary>
        public void ToRegister();

        /// <summary>
        /// Function used to redirect to /account/orders
        /// </summary>
        public void ToOrders();

        /// <summary>
        /// Function used to redirect to /account/routes
        /// </summary>
        public void ToRoutes();

        /// <summary>
        /// Function used to redirect to project's github page
        /// </summary>
        public Task ToGithubAsync();
    }
}
