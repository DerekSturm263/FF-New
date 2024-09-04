

float dot2(float2 a){
	return dot(a,a);
}


float RectangleSDF(float2 p, float2 rectSize)
{
   // Translate the pot p so that the rectangle is centered at (0.5, 0.5)
	p -= float2(0.5, 0.5);

    // Compute the signed distance to the nearest edge
	float2 d = abs(p) - rectSize;

    // The distance side the rectangle
	float sideDist =  max(d.x, d.y);

    // The distance outside the rectangle
	float outsideDist = length(max(d, 0.0));

    // Combe the distances to get the signed distance
	return sideDist < 0.0 ? sideDist : outsideDist;
}

float DiamondSDF(float2 p, float2 rectSize)
{
	p -= float2(0.5, 0.5);
    // Rotate the coordate system by 45 degrees
	p = float2(p.x + p.y, p.x - p.y) * 0.70710678; // sqrt(2)/2

    // Compute the signed distance to the diamond (rotated square)
	float2 d = abs(p) - rectSize;
	float sideDist = max(d.x, d.y);
	float outsideDist = length(max(d, 0.0));

	return sideDist < 0.0 ? sideDist : outsideDist;
}




float sdCircle(float2 p, float r)
{
	return length(p) - r;
}


float sdRoundedBox( float2 p,  float2 b,  float4 r)
{
	r.xy = (p.x > 0.0) ? r.xy : r.zw;
	r.x = (p.y > 0.0) ? r.x : r.y;
	float2 q = abs(p) - b + r.x;
	return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - r.x;
}



float sdBox( float2 p,  float2 b)
{
	float2 d = abs(p) - b;
	return length(max(d, 0.0)) + min(max(d.x, d.y), 0.0);
}



float sdOrientedBox( float2 p,  float2 a,  float2 b, float th)
{
	float l = length(b - a);
	float2 d = (b - a) / l;
	float2 q = (p - (a + b) * 0.5);
	q = mul(float2x2(d.x, -d.y, d.y, d.x),q);
	q = abs(q) - float2(l, th) * 0.5;
	return length(max(q, 0.0)) + min(max(q.x, q.y), 0.0);
}



float sdSegment( float2 p,  float2 a,  float2 b)
{
	float2 pa = p - a, ba = b - a;
	float h = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
	return length(pa - ba * h);
}



float ndot(float2 a, float2 b)
{
	return a.x * b.x - a.y * b.y;
}
float sdRhombus( float2 p,  float2 b)
{
	p = abs(p);
	float h = clamp(ndot(b - 2.0 * p, b) / dot(b, b), -1.0, 1.0);
	float d = length(p - 0.5 * b * float2(1.0 - h, 1.0 + h));
	return d * sign(p.x * b.y + p.y * b.x - b.x * b.y);
}



float sdTrapezoid( float2 p,  float r1, float r2, float he)
{
	float2 k1 = float2(r2, he);
	float2 k2 = float2(r2 - r1, 2.0 * he);
	p.x = abs(p.x);
	float2 ca = float2(p.x - min(p.x, (p.y < 0.0) ? r1 : r2), abs(p.y) - he);
	float2 cb = p - k1 + k2 * clamp(dot(k1 - p, k2) / dot2(k2), 0.0, 1.0);
	float s = (cb.x < 0.0 && ca.y < 0.0) ? -1.0 : 1.0;
	return s * sqrt(min(dot2(ca), dot2(cb)));
}


float sdParallelogramin( float2 p, float wi, float he, float sk)
{
	float2 e = float2(sk, he);
	p = (p.y < 0.0) ? -p : p;
	float2 w = p - e;
	w.x -= clamp(w.x, -wi, wi);
	float2 d = float2(dot(w, w), -w.y);
	float s = p.x * e.y - p.y * e.x;
	p = (s < 0.0) ? -p : p;
	float2 v = p - float2(wi, 0);
	v -= e * clamp(dot(v, e) / dot(e, e), -1.0, 1.0);
	d = min(d, float2(dot(v, v), wi * he - abs(s)));
	return sqrt(d.x) * sign(-d.y);
}



float sdEquilateralTriangle( float2 p,  float r)
{
	const float k = sqrt(3.0);
	p.x = abs(p.x) - r;
	p.y = p.y + r / k;
	if (p.x + k * p.y > 0.0)
		p = float2(p.x - k * p.y, -k * p.x - p.y) / 2.0;
	p.x -= clamp(p.x, -2.0 * r, 0.0);
	return -length(p) * sign(p.y);
}



float sdTriangleIsoscelesin( float2 p,  float2 q)
{
	p.x = abs(p.x);
	float2 a = p - q * clamp(dot(p, q) / dot(q, q), 0.0, 1.0);
	float2 b = p - q * float2(clamp(p.x / q.x, 0.0, 1.0), 1.0);
	float s = -sign(q.y);
	float2 d = min(float2(dot(a, a), s * (p.x * q.y - p.y * q.x)),
                  float2(dot(b, b), s * (p.y - q.y)));
	return -sqrt(d.x) * sign(d.y);
}



float sdTriangle( float2 p,  float2 p0,  float2 p1,  float2 p2)
{
	float2 e0 = p1 - p0, e1 = p2 - p1, e2 = p0 - p2;
	float2 v0 = p - p0, v1 = p - p1, v2 = p - p2;
	float2 pq0 = v0 - e0 * clamp(dot(v0, e0) / dot(e0, e0), 0.0, 1.0);
	float2 pq1 = v1 - e1 * clamp(dot(v1, e1) / dot(e1, e1), 0.0, 1.0);
	float2 pq2 = v2 - e2 * clamp(dot(v2, e2) / dot(e2, e2), 0.0, 1.0);
	float s = sign(e0.x * e2.y - e0.y * e2.x);
	float2 d = min(min(float2(dot(pq0, pq0), s * (v0.x * e0.y - v0.y * e0.x)),
                     float2(dot(pq1, pq1), s * (v1.x * e1.y - v1.y * e1.x))),
                     float2(dot(pq2, pq2), s * (v2.x * e2.y - v2.y * e2.x)));
	return -sqrt(d.x) * sign(d.y);
}



float sdUnevenCapsule(float2 p, float r1, float r2, float h)
{
	p.x = abs(p.x);
	float b = (r1 - r2) / h;
	float a = sqrt(1.0 - b * b);
	float k = dot(p, float2(-b, a));
	if (k < 0.0)
		return length(p) - r1;
	if (k > a * h)
		return length(p - float2(0.0, h)) - r2;
	return dot(p, float2(a, b)) - r1;
}


float sdPentagon( float2 p,  float r)
{
	const float3 k = float3(0.809016994, 0.587785252, 0.726542528);
	p.x = abs(p.x);
	p -= 2.0 * min(dot(float2(-k.x, k.y), p), 0.0) * float2(-k.x, k.y);
	p -= 2.0 * min(dot(float2(k.x, k.y), p), 0.0) * float2(k.x, k.y);
	p -= float2(clamp(p.x, -r * k.z, r * k.z), r);
	return length(p) * sign(p.y);
}


float sdHexagon( float2 p,  float r)
{
	const float3 k = float3(-0.866025404, 0.5, 0.577350269);
	p = abs(p);
	p -= 2.0 * min(dot(k.xy, p), 0.0) * k.xy;
	p -= float2(clamp(p.x, -k.z * r, k.z * r), r);
	return length(p) * sign(p.y);
}


float sdOctogon( float2 p,  float r)
{
	const float3 k = float3(-0.9238795325, 0.3826834323, 0.4142135623);
	p = abs(p);
	p -= 2.0 * min(dot(float2(k.x, k.y), p), 0.0) * float2(k.x, k.y);
	p -= 2.0 * min(dot(float2(-k.x, k.y), p), 0.0) * float2(-k.x, k.y);
	p -= float2(clamp(p.x, -k.z * r, k.z * r), r);
	return length(p) * sign(p.y);
}

float sdHexagramin( float2 p,  float r)
{
	const float4 k = float4(-0.5, 0.8660254038, 0.5773502692, 1.7320508076);
	p = abs(p);
	p -= 2.0 * min(dot(k.xy, p), 0.0) * k.xy;
	p -= 2.0 * min(dot(k.yx, p), 0.0) * k.yx;
	p -= float2(clamp(p.x, r * k.z, r * k.w), r);
	return length(p) * sign(p.y);
}



float sdStar( float2 p,  float r,  float rf)
{
	const float2 k1 = float2(0.809016994375, -0.587785252292);
	const float2 k2 = float2(-k1.x, k1.y);
	p.x = abs(p.x);
	p -= 2.0 * max(dot(k1, p), 0.0) * k1;
	p -= 2.0 * max(dot(k2, p), 0.0) * k2;
	p.x = abs(p.x);
	p.y -= r;
	float2 ba = rf * float2(-k1.y, k1.x) - float2(0, 1);
	float h = clamp(dot(p, ba) / dot(ba, ba), 0.0, r);
	return length(p - ba * h) * sign(p.y * ba.x - p.x * ba.y);
}




float sdRing( float2 p,  float2 n,  float r, float th)
{
	p.x = abs(p.x);
   
	p = mul(float2x2(n.x, n.y, -n.y, n.x) ,p);

	return max(abs(length(p) - r) - th * 0.5,
                length(float2(p.x, max(0.0, abs(r - p.y) - th * 0.5))) * sign(p.x));
}







float sdHeart( float2 p)
{
	p.x = abs(p.x);

	if (p.y + p.x > 1.0)
		return sqrt(dot2(p - float2(0.25, 0.75))) - sqrt(2.0) / 4.0;
	return sqrt(min(dot2(p - float2(0.00, 1.00)),
                    dot2(p - 0.5 * max(p.x + p.y, 0.0)))) * sign(p.x - p.y);
}



