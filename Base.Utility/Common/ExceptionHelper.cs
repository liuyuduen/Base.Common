using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Base.Utility.Common
{
    public class ExceptionHelper
    {
        public static bool TryExec(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                // 保存日志
            }
            return false;
        }

        public static bool TryExec(Action action, Action<Exception> actionFailed)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                actionFailed(ex);
            }
            return false;
        }
    }

    public class TestExcetion
    {
        /// <summary>
        /// 这样处理异常的意义是什么？
        /// </summary>
        public void Test()
        {
            ExceptionHelper.TryExec(() =>
            {
                // 要执行的操作
            });

            ExceptionHelper.TryExec(() =>
            {
                // 要执行的操作
            },

            (ex) =>
            {
                // 出错后要执行的操作
            });
        }
      
    }
}
