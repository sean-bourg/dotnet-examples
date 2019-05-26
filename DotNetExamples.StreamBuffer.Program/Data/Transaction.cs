using System;

namespace DotNetExamples.StreamBuffer.Program.Data
{
    public readonly struct Transaction : IEquatable<Transaction>
    {
        /// <summary>
        /// Transaction id number.
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// Account id number.
        /// </summary>
        public readonly string Account;

        /// <summary>
        /// Amount of this transaction.
        /// </summary>
        public readonly decimal Amount;

        /// <summary>
        /// Timestamp this transaction occurred.
        /// </summary>
        public readonly DateTime Timestamp;

        /// <summary>
        /// Construct transaction instance.
        /// </summary>
        /// <param name="id">Transaction Id number.</param>
        /// <param name="account">Account number.</param>
        /// <param name="amount">Amount of transaction.</param>
        /// <param name="timestamp">Transaction occurance time.</param>
        public Transaction(int id, string account, decimal amount, DateTime timestamp)
        {
            Id = id;
            Account = account;
            Amount = amount;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public bool Equals(Transaction obj) => Id == obj.Id;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj) => (obj is Transaction) ? this.Equals((Transaction)obj) : false;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public static bool operator ==(Transaction t1, Transaction t2) => t1.Equals(t2);

        /// <summary>
        /// Determines whether the specified object is not equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public static bool operator !=(Transaction t1, Transaction t2) => !t1.Equals(t2);

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => Id.GetHashCode();

        /// <summary>
        /// Convert a transaction to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => String.Format("Id={0}, Account={1}, Amount={2:C}, Timestamp={3}", Id, Account, Amount, Timestamp.ToString("yyyy-MM-dd HH:mm:ss.ff"));
    }
}
