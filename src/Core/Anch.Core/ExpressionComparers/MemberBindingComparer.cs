using System.Linq.Expressions;

namespace Anch.Core.ExpressionComparers;

public class MemberBindingComparer(ExpressionComparer rootComparer) : IEqualityComparer<MemberBinding>
{
	private readonly ElementInitComparer elementInitComparer = new (rootComparer);

	public bool Equals(MemberBinding? preX, MemberBinding? preY)
    {
        if (ReferenceEquals(preX, preY)) return true;
        if (preX is null || preY is null) return false;

        var baseEquals = preX.BindingType == preY.BindingType && preX.Member == preY.Member;

        return baseEquals &&

               (from x in (preX as MemberAssignment).ToMaybe()

                 from y in (preY as MemberAssignment).ToMaybe()

                 select rootComparer.Equals(x.Expression, y.Expression))

               .Or(() => from x in (preX as MemberListBinding).ToMaybe()

                         from y in (preY as MemberListBinding).ToMaybe()

                         select x.Initializers.SequenceEqual(y.Initializers, this.elementInitComparer))

               .Or(() => from x in (preX as MemberMemberBinding).ToMaybe()

                         from y in (preY as MemberMemberBinding).ToMaybe()

                         select x.Bindings.SequenceEqual(y.Bindings, this))

               .GetValueOrDefault();
    }

    public int GetHashCode(MemberBinding memberBinding)
    {
        return memberBinding.BindingType.GetHashCode() ^ memberBinding.Member.GetHashCode();
    }
}
