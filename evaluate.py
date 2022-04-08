#!/usr/bin/env python3.7
l='#'
k=open
j=str
T='user'
S='sat'
R=''
Q=float
M='Invalid line! '
L='interferers'
K=range
H='sats'
G=True
F=len
E='users'
B=False
A=print
import argparse as U,sys
from collections import namedtuple as D
from math import sqrt as N,acos,degrees as V,floor
C=D('Vector3',['x','y','z'])
W=C(0,0,0)
X=32
Y=[j(A)for A in K(1,X+1)]
Z=4
a=[chr(ord('A')+A)for A in K(0,Z)]
b=10.0
c=20.0
O=45.0
def J(vertex,point_a,point_b):
	G=point_b;F=point_a;B=vertex;D=C(F.x-B.x,F.y-B.y,F.z-B.z);E=C(G.x-B.x,G.y-B.y,G.z-B.z);H=N(D.x**2+D.y**2+D.z**2);I=N(E.x**2+E.y**2+E.z**2);J=C(D.x/H,D.y/H,D.z/H);K=C(E.x/I,E.y/I,E.z/I);L=J.x*K.x+J.y*K.y+J.z*K.z;M=min(1.0,max(-1.0,L))
	if abs(M-L)>1e-06:A(f"dot_product: {L} bounded to {M}")
	return V(acos(M))
def d(scenario,solution):
	O=solution;L=scenario;A('Checking no sat interferes with itself...')
	for M in O:
		C=O[M];D=list(C.keys());Q=L[H][M]
		for I in K(F(C)):
			for N in K(I+1,F(C)):
				R=C[D[I]][1];S=C[D[N]][1]
				if R!=S:continue
				T=C[D[I]][0];U=C[D[N]][0];V=L[E][T];W=L[E][U];P=J(Q,V,W)
				if P<b:A(f"\tSat {M} beams {D[I]} and {D[N]} interfere.");A(f"\t\tBeam angle: {P} degrees.");return B
	A('\tNo satellite self-interferes.');return G
def e(scenario,solution):
	F=solution;C=scenario;A('Checking no sat interferes with a non-Starlink satellite...')
	for D in F:
		N=C[H][D]
		for I in F[D]:
			O=F[D][I][0];P=C[E][O]
			for K in C[L]:
				Q=C[L][K];M=J(P,N,Q)
				if M<c:A(f"\tSat {D} beam {I} interferes with non-Starlink sat {K}.");A(f"\t\tAngle of separation: {M} degrees.");return B
	A('\tNo satellite interferes with a non-Starlink satellite!');return G
def f(scenario,solution):
	C=solution;A('Checking user coverage...');D=[]
	for I in C:
		for K in C[I]:
			H=C[I][K][0]
			if H in D:A(f"\tUser {H} is covered multiple times by solution!");return B
			D.append(H)
	J=F(scenario[E]);L=F(D);A(f"{L/J*100}% of {J} total users covered.");return G
def g(scenario,solution):
	F=scenario;D=solution;A('Checking each user can see their assigned satellite...')
	for C in D:
		for L in D[C]:
			I=D[C][L][0];M=F[E][I];N=F[H][C];K=J(M,W,N)
			if K<=180.0-O:P=j(K-90);A(f"\tSat {C} outside of user {I}'s field of view.");A(f"\t\t{P} degrees elevation.");A(f"\t\t(Min: {90-O} degrees elevation.)");return B
	A("\tAll users' assigned satellites are visible.");return G
def I(object_type,line,dest):
	E=line;D=E.split()
	if D[0]!=object_type or F(D)!=5:A(M+E);return B
	else:
		H=D[1]
		try:I=Q(D[2]);J=Q(D[3]);K=Q(D[4])
		except:A("Can't parse location! "+E);return B
		dest[H]=C(I,J,K);return G
def h(filename,scenario):
	K='interferer';F=filename;D=scenario;A('Reading scenario file '+F);J=k(F).readlines();D[H]={};D[E]={};D[L]={}
	for C in J:
		if l in C:continue
		elif C.strip()==R:continue
		elif K in C:
			if not I(K,C,D[L]):return B
		elif S in C:
			if not I(S,C,D[H]):return B
		elif T in C:
			if not I(T,C,D[E]):return B
		else:A(M+C);return B
	return G
def P(filename,scenario,solution):
	O=scenario;K=filename;J=solution
	if K==R:A('Reading solution from stdin.');L=sys.stdin
	else:A(f"Reading solution file {K}.");L=k(K)
	U=L.readlines()
	for D in U:
		C=D.split()
		if l in D:continue
		elif F(C)==0:continue
		elif F(C)==8:
			if C[0]!=S or C[2]!='beam'or C[4]!=T or C[6]!='color':A(M+D);return B
			I=C[1];N=C[3];P=C[5];Q=C[7]
			if not I in O[H]:A('Referenced an invalid sat id! '+D);return B
			if not P in O[E]:A('Referenced an invalid user id! '+D);return B
			if not N in Y:A('Referenced an invalid beam id! '+D);return B
			if not Q in a:A('Referenced an invalid color! '+D);return B
			if not I in J:J[I]={}
			if N in J[I]:A('Beam is allocated multiple times! '+D);return B
			J[I][N]=P,Q
		else:A(M+D);return B
	L.close();return G
def i():
	D=U.ArgumentParser(prog=f"python3.7 {sys.argv[0]}",description='Starlink beam-planning evaluation tool');D.add_argument('scenario',metavar='/path/to/scenario.txt',help='Test input scenario.');D.add_argument('solution',metavar='/path/to/solution.txt',nargs='?',help='Optional. If not provided, stdin will be read.');E=D.parse_args();B={}
	if not h(E.scenario,B):return-1
	C={}
	if E.solution is None:
		if not P(R,B,C):return-1
	elif not P(E.solution,B,C):return-1
	if not f(B,C):return-1
	if not g(B,C):return-1
	if not d(B,C):return-1
	if not e(B,C):A('Solution contained a beam that could interfere with a non-Starlink satellite.');return-1
	A('\nSolution passed all checks!\n');return 0
if __name__=='__main__':exit(i())