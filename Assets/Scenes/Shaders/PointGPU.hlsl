/*

	This file is used by PointURP_GPU.shadergraph.
	S1. add a "Custom Function" node in shadergraph
	S2. the node will invoke the custom functions below via reading this file.
	
	NOTE: 1. The following functions are similar to function overloading in C++.
		  Thus They must have the same prefix but with different sufix for different 
		  data types.

		  2. The function prefix must be the same as the "Custom Function Node" name 
		  in shader graph.

*/

#include "PointURP_GPU.hlsl"

void ShaderGraphFunction_float (float3 In, out float3 Out) {	
	Out = In;
}

// this version is for half type data
void ShaderGraphFunction_half (half3 In, out half3 Out) {
	Out = In;
}
