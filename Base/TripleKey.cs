using System;

namespace WpfApp1.Base
{
    public class TripleKey : IEquatable<TripleKey>
    {
        public TripleKey(Guid trusteeId, Guid objectId, Guid typeId)
        {
            TrusteeId = trusteeId;
            ObjectId = objectId;
            TypeId = typeId;
        }

        public Guid TrusteeId { get; }
        public Guid ObjectId { get; }
        public Guid TypeId { get; }

        public bool Equals(TripleKey other)
        {
            return other.TrusteeId.Equals(TrusteeId)
                && other.ObjectId.Equals(ObjectId)
                && other.TypeId.Equals(TypeId);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TripleKey);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = TrusteeId.GetHashCode();
                hashCode = (hashCode * 397) ^ ObjectId.GetHashCode();
                hashCode = (hashCode * 397) ^ TypeId.GetHashCode();
                return hashCode;
            }
        }
    }
}
