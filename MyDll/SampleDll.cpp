#include <stdio.h>
#include <string.h>
#include "SampleDll.h"

namespace MyDll {
	
	//intを受け取る関数
	int __stdcall MyFuncA(int a) {
		printf("-- < MyFunc > --\n");
		printf("int a = %d\n", a);
		printf("---------------\n");
		return a + 1;
	}
	//文字列を受け取る関数
	void __stdcall MyFuncB(int a, char* str) {
		printf("--< MyFuncB >--\n");
		printf("[%d] %s\n", a, str);
		printf("---------------\n");
	}
	//引数で受け取った文字列に値をセットする
	void __stdcall MyFuncC(int a, char* str) {
		sprintf_s(str, 256, "Set By MyFuncC");
		printf("--< MyFuncC >--\n");
		printf("[%d] %s\n", a, str);
		printf("---------------\n");
	}
	//構造体を受け取る関数
	void __stdcall MyFuncD(SampleStruct st) {
		printf("--< MyFuncD >--\n");
		printf("index = %d\n", st.index);
		printf("name = %s\n", st.name);
		printf("data = {%d, %d, %d}\n", st.data[0], st.data[1], st.data[2]);
		printf("---------------\n");
	}
	//構造体を受け取り、操作する関数
	void __stdcall MyFuncE(SampleStruct* pst) {
		printf("--< MyFuncE >--\n");
		(*pst).index = 55;
		sprintf_s((*pst).name, "構造体ポインタサンプル");
		(*pst).data[0] = 51;
		(*pst).data[1] = 52;
		(*pst).data[2] = 53;
		printf("---------------\n");
	}
	//メンバにポインタをもつ構造体を受け取る関数
	void __stdcall MyFuncF(SampleStruct2* st2) {
		double g_dData[256];
		printf("--< MyFuncF >--\n");
		memset(st2, 0, sizeof(SampleStruct2));
		memset(g_dData, 0, sizeof(g_dData));
		(*st2).length = 10;
		(*st2).data = g_dData;
		for (int i = 0; i < (*st2).length; i++) {
			g_dData[i] = 600 + (i + 1) / 10.0;
		}
		printf("---------------\n");
	}
	//intのポインタを受け取る関数
	void __stdcall MyFuncG(int* a) {
		printf("--< MyFuncG >--\n");
		int x = 100;
		a = &x;
		printf("a = %d\n", *a);
		*a = 200;
		printf("a = %d\n", *a);
		printf("---------------\n");
	}
	//intのポインタのポインタを受け取る関数
	void __stdcall MyFuncH(int** a) {
		printf("--< MyFuncH >--\n");
		int* ptr;
		int data = 1000;
		ptr = &data;
		a = &ptr;
		printf("a = %d\n", **a);
		**a = 2000;
		printf("a = %d\n", **a);
		printf("---------------\n");
	}
	//構造体SampleStructのポインタのポインタを受け取る関数
	void __stdcall MyFuncI(SampleStruct** ppst) {
		printf("--< MyFuncI >--\n");
		(**ppst).index = 70;
		sprintf_s((**ppst).name, "構造体ポインタのポインタのサンプル");
		(**ppst).data[0] = 11;
		(**ppst).data[1] = 22;
		(**ppst).data[2] = 33;
		printf("---------------\n");
	}

}