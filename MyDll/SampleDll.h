#pragma once

typedef struct _SampleStruct {
	int index;
	char name[128];
	int data[50];
}SampleStruct, *PSampleStruct;

typedef struct _SampleStruct2 {
	int length;
	double* data;
}SampleStruct2, *PSampleStruct2;

namespace MyDll {
	int __stdcall MyFuncA(int a);
	void __stdcall MyFuncB(int a, char* str);
	void __stdcall MyFuncC(int a, char* str);
	void __stdcall MyFuncD(SampleStruct st);
	void __stdcall MyFuncE(SampleStruct* pst);
	void __stdcall MyFuncF(SampleStruct2* st2);
	void __stdcall MyFuncG(int* a);
	void __stdcall MyFuncH(int** a);
	void __stdcall MyFuncI(SampleStruct** ppst);

}