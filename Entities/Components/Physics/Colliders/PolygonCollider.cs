﻿using System;
using Microsoft.Xna.Framework;
using Nocturne.PhysicsShapes;


namespace Nocturne
{
	/// <summary>
	/// Polygons should be defined in clockwise fashion.
	/// </summary>
	public class PolygonCollider : Collider
	{
		/// <summary>
		/// If the points are not centered they will be centered with the difference being applied to the localOffset.
		/// </summary>
		/// <param name="points">Points.</param>
		public PolygonCollider( Vector2[] points )
		{
			// first and last point must not be the same. we want an open polygon
			var isPolygonClosed = points[0] == points[points.Length - 1];

			if ( isPolygonClosed )
				Array.Resize( ref points, points.Length - 1 );

			var center = Polygon.FindPolygonCenter( points );
			SetLocalOffset( center );
			Polygon.RecenterPolygonVerts( points );
			Shape = new Polygon( points );
		}

		public PolygonCollider( int vertCount, float radius )
		{
			Shape = new Polygon( vertCount, radius );
		}

		public PolygonCollider() : this( 6, 40 )
		{
		}

		
		public override void DebugRender()
		{
			var poly = Shape as Polygon;
			Draw.HollowRect( Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height, Debug.Colors.ColliderBounds );
			Draw.Polygon( Shape.position, poly.Points, Debug.Colors.ColliderEdge, true);
			Draw.Point( Entity.Transform.Position, Debug.Colors.ColliderPosition );
			Draw.Point( Shape.position, Debug.Colors.ColliderCenter );

			// Normal debug code
			//for( var i = 0; i < poly.points.Length; i++ )
			//{
			//	Vector2 p2;
			//	var p1 = poly.points[i];
			//	if( i + 1 >= poly.points.Length )
			//		p2 = poly.points[0];
			//	else
			//		p2 = poly.points[i + 1];
			//	var perp = Vector2Ext.perpendicular( ref p1, ref p2 );
			//	Vector2Ext.normalize( ref perp );
			//	var mp = Vector2.Lerp( p1, p2, 0.5f ) + poly.position;
			//	batcher.drawLine( mp, mp + perp * 10, Color.White );
			//}
		}
		
	}
}