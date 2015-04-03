namespace DataCommander.Foundation
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IIntervalComparer<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [Pure]
        bool IsValid(T left, T right);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="value"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        [Pure]
        bool Contains(T left, T value, T right);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left1"></param>
        /// <param name="right1"></param>
        /// <param name="left2"></param>
        /// <param name="right2"></param>
        /// <returns></returns>
        [Pure]
        bool Intersects(T left1, T right1, T left2, T right2);
    }
}