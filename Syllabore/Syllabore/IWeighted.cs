using System;
using System.Collections.Generic;
using System.Text;

namespace Syllabore
{
    /// <summary>
    /// <para>
    /// Represents a choice or entity that can
    /// be randomly selected from a list, and has 
    /// a weight value that affects how frequently it
    /// is selected compared to others.
    /// </para>
    /// <para>
    /// For example, if a list contains two elements x and y, 
    /// both of type <see cref="IWeighted"/>, with respective
    /// weights of 1 and 4, then
    /// y will be selected four times as likely as x.
    /// </para>
    /// </summary>
    public interface IWeighted
    {
        /// <summary>
        /// <para>
        /// A value indicating how more frequently this instance of 
        /// <see cref="IWeighted"/> should be selected over other instances.
        /// </para>
        /// <para>
        /// For example, if a list contains two elements x and y, 
        /// both of type <see cref="IWeighted"/>, with respective
        /// weights of 1 and 4, then
        /// y will be selected four times as likely as x.
        /// </para>
        /// </summary>
        int Weight { get; set; }
    }
}
