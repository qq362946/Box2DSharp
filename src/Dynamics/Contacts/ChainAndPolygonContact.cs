using System.Diagnostics;
using Box2DSharp.Collision;
using Box2DSharp.Collision.Collider;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Microsoft.Extensions.ObjectPool;

namespace Box2DSharp.Dynamics.Contacts
{
    public class ChainAndPolygonContact : Contact
    {
        private static readonly ObjectPool<ChainAndPolygonContact> _pool =
            new DefaultObjectPool<ChainAndPolygonContact>(new PoolPolicy());

        internal static Contact Create(Fixture fixtureA, int indexA, Fixture fixtureB, int indexB)
        {
            Debug.Assert(fixtureA.ShapeType == ShapeType.Chain);
            Debug.Assert(fixtureB.ShapeType == ShapeType.Polygon);
            var contact = _pool.Get();
            contact.Initialize(fixtureA, indexA, fixtureB, indexB);
            return contact;
        }

        public static void Destroy(Contact contact)
        {
            _pool.Return((ChainAndPolygonContact) contact);
        }

        internal override void Evaluate(ref Manifold manifold, in Transform xfA, Transform xfB)
        {
            var chain = (ChainShape) FixtureA.Shape;

            chain.GetChildEdge(out var edge, IndexA);
            CollisionUtils.CollideEdgeAndPolygon(ref manifold, edge, xfA, (PolygonShape) FixtureB.Shape, xfB);
        }

        private class PoolPolicy : IPooledObjectPolicy<ChainAndPolygonContact>
        {
            public ChainAndPolygonContact Create()
            {
                return new ChainAndPolygonContact();
            }

            public bool Return(ChainAndPolygonContact obj)
            {
                obj.Reset();
                return true;
            }
        }
    }
}