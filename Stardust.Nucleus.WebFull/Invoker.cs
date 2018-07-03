using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Stardust.Particles;

namespace Stardust.Nucleus.Web
{
    public class ControllerInvoker : ControllerActionInvoker
    {
        public static Action<IDependencyResolver, ControllerContext> OnControllerActionCompleted;
        protected override ActionResult InvokeActionMethod(ControllerContext controllerContext, ActionDescriptor actionDescriptor, IDictionary<string, object> parameters)
        {
            IDependencyResolver resolver = null;
            try
            {
                resolver = HttpContext.Current.Items["resolver"] as IDependencyResolver ?? Resolver.CreateScopedResolver();
                var logger = resolver.GetService<ILogging>();
                using (logger.CreateLogScope($"Invoking action {actionDescriptor.ActionName}"))
                {
                    var result = base.InvokeActionMethod(controllerContext, actionDescriptor, parameters);
                    OnControllerActionCompleted?.Invoke(resolver, controllerContext);
                    return result;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                throw;
            }
            finally
            {
                resolver.TryDispose();
            }
        }
    }

    public class Invoker : ApiControllerActionInvoker
    {
        public static Action<IDependencyResolver, HttpActionContext> OnApiActionCompleted;

        public override async Task<HttpResponseMessage> InvokeActionAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            IDependencyResolver resolver = null;
            try
            {
                resolver = HttpContext.Current.Items["resolver"] as IDependencyResolver;
                if (resolver == null) resolver = Resolver.CreateScopedResolver();
                HttpContext.Current.Items.Add("logContextName",
                    actionContext.ActionDescriptor.ControllerDescriptor.ControllerName);
                var logger = resolver.GetService<ILogging>();

                using (logger.CreateLogScope($"Invoking action {actionContext.ActionDescriptor.ActionName}"))
                {
                    var result = await base.InvokeActionAsync(actionContext, cancellationToken);
                    OnApiActionCompleted?.Invoke(resolver, actionContext);
                    return result;
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                throw;
            }
            finally
            {
                resolver.TryDispose();
            }
        }
    }

    public static class LogScopeCreator
    {
        public static LogScope CreateLogScope(this ILogging logger, string message, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null, [CallerFilePath] string filepaht = null)
        {
            return new LogScope(message, logger, lineNumber, caller, filepaht);
        }

        public static LogScope CreateLogScope(this ILogging logger, [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null, [CallerFilePath] string filepaht = null)
        {
            return new LogScope(null, logger, lineNumber, caller, filepaht);
        }
    }

    public sealed class LogScope : IDisposable
    {
        private readonly ILogging _logging;
        private readonly int _lineNumber;
        private readonly string _caller;
        private readonly string _filepaht;
        private readonly string _message;
        private Stopwatch _timer;

        internal LogScope(string message, ILogging logging, int lineNumber = 0, string caller = null, string filepaht = null)
        {
            _logging = logging;
            _lineNumber = lineNumber;
            _caller = caller;
            _filepaht = filepaht;
            _message = $"{message}";
            _timer = Stopwatch.StartNew();
            _logging?.DebugMessage($"Beginning action {caller} at line {lineNumber} in {filepaht}");
            if (message.ContainsCharacters())
                _logging?.DebugMessage(message);
        }
        public void Dispose()
        {
            _timer.Stop();
            _logging?.DebugMessage($"ending  action {_caller} at line {_lineNumber} in {_filepaht}. Execution time {_timer.ElapsedMilliseconds}ms");
        }
    }
}