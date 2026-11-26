#include "../encryption/EncryptionVM.h"

#include "vm/Exception.h"

namespace hybridclr
{
namespace encryption
{
	void EncryptionVM::Decrypt(const void* ops, size_t opLength, const byte* key, byte* data, uint32_t dataLength)
	{
		if (dataLength == 0)
		{
			return;
		}
		for (size_t i = 0; i < opLength; i++)
		{
			byte op = ((byte*)ops)[i];
			switch (op)
			{
				//!!!{{INSTRUCTIONS
			// Seed:0
			case 0:{ uint32_t xIndex1 = 159719620 % dataLength; uint32_t xIndex2 = ((uint32_t)key[119] + 773150046u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 1:{ uint32_t xIndex1 = 2727824503 % dataLength; uint32_t xIndex2 = ((uint32_t)key[94] + 548247209u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 2:{ uint32_t xIndex = 773150046 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[169] ^ 200u); }break;
			case 3:{ uint32_t xIndex1 = 548247209 % dataLength; uint32_t xIndex2 = ((uint32_t)key[200] + 2832368235u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 4:{ uint32_t xIndex = 2115878600 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[107] - 130u); }break;
			case 5:{ uint32_t xIndex = 2832368235 % dataLength; uint32_t shift = (uint32_t)(key[130] + 2531105853) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 6:{ uint32_t xIndex = 2006221698 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[61] ^ 12u); }break;
			case 7:{ uint32_t xIndex = 2531105853 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[12] ^ 159u); }break;
			case 8:{ uint32_t xIndex1 = 3989110284 % dataLength; uint32_t xIndex2 = ((uint32_t)key[159] + 2165923046u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 9:{ uint32_t xIndex1 = 2222380191 % dataLength; uint32_t xIndex2 = ((uint32_t)key[230] + 1345953809u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 10:{ uint32_t xIndex = 2165923046 % dataLength; uint32_t shift = (uint32_t)(key[17] + 1043415696) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 11:{ uint32_t xIndex = 1345953809 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[144] - 19u); }break;
			case 12:{ uint32_t xIndex = 1043415696 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[19] - 138u); }break;
			case 13:{ uint32_t xIndex = 586225427 % dataLength; uint32_t shift = (uint32_t)(key[138] + 2343900709) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 14:{ uint32_t xIndex = 3870123402 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[37] - 84u); }break;
			case 15:{ uint32_t xIndex1 = 2343900709 % dataLength; uint32_t xIndex2 = ((uint32_t)key[84] + 3522190791u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 16:{ uint32_t xIndex1 = 3109564500 % dataLength; uint32_t xIndex2 = ((uint32_t)key[199] + 2090033518u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 17:{ uint32_t xIndex = 3522190791 % dataLength; uint32_t shift = (uint32_t)(key[110] + 3566711417) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 18:{ uint32_t xIndex = 2090033518 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[121] ^ 88u); }break;
			case 19:{ uint32_t xIndex = 3566711417 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[88] - 187u); }break;
			case 20:{ uint32_t xIndex1 = 3845303128 % dataLength; uint32_t xIndex2 = ((uint32_t)key[187] + 2234209426u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 21:{ uint32_t xIndex1 = 3357146299 % dataLength; uint32_t xIndex2 = ((uint32_t)key[146] + 4040255757u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 22:{ uint32_t xIndex1 = 2234209426 % dataLength; uint32_t xIndex2 = ((uint32_t)key[13] + 146855836u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 23:{ uint32_t xIndex = 4040255757 % dataLength; uint32_t shift = (uint32_t)(key[156] + 2784856047) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 24:{ uint32_t xIndex = 146855836 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[239] - 246u); }break;
			case 25:{ uint32_t xIndex1 = 2784856047 % dataLength; uint32_t xIndex2 = ((uint32_t)key[246] + 2307812833u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 26:{ uint32_t xIndex = 4262888182 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[225] - 32u); }break;
			case 27:{ uint32_t xIndex1 = 2307812833 % dataLength; uint32_t xIndex2 = ((uint32_t)key[32] + 2202588003u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 28:{ uint32_t xIndex1 = 2186156320 % dataLength; uint32_t xIndex2 = ((uint32_t)key[99] + 1218146458u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 29:{ uint32_t xIndex1 = 2202588003 % dataLength; uint32_t xIndex2 = ((uint32_t)key[154] + 3255514357u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 30:{ uint32_t xIndex = 1218146458 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[245] ^ 228u); }break;
			case 31:{ uint32_t xIndex1 = 3255514357 % dataLength; uint32_t xIndex2 = ((uint32_t)key[228] + 1547432727u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 32:{ uint32_t xIndex1 = 1391793124 % dataLength; uint32_t xIndex2 = ((uint32_t)key[23] + 2974409086u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 33:{ uint32_t xIndex1 = 1547432727 % dataLength; uint32_t xIndex2 = ((uint32_t)key[126] + 816345673u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 34:{ uint32_t xIndex = 2974409086 % dataLength; uint32_t shift = (uint32_t)(key[73] + 2204347368) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 35:{ uint32_t xIndex1 = 816345673 % dataLength; uint32_t xIndex2 = ((uint32_t)key[232] + 4082973451u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 36:{ uint32_t xIndex = 2204347368 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[11] - 162u); }break;
			case 37:{ uint32_t xIndex1 = 4082973451 % dataLength; uint32_t xIndex2 = ((uint32_t)key[162] + 3997112797u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 38:{ uint32_t xIndex1 = 2598295970 % dataLength; uint32_t xIndex2 = ((uint32_t)key[221] + 1172243756u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 39:{ uint32_t xIndex = 3997112797 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[44] - 63u); }break;
			case 40:{ uint32_t xIndex = 1172243756 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[63] ^ 6u); }break;
			case 41:{ uint32_t xIndex1 = 2070757183 % dataLength; uint32_t xIndex2 = ((uint32_t)key[6] + 2947833777u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 42:{ uint32_t xIndex = 1349033222 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[177] - 176u); }break;
			case 43:{ uint32_t xIndex1 = 2947833777 % dataLength; uint32_t xIndex2 = ((uint32_t)key[176] + 2644169651u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 44:{ uint32_t xIndex1 = 4186407856 % dataLength; uint32_t xIndex2 = ((uint32_t)key[179] + 3265965994u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 45:{ uint32_t xIndex1 = 2644169651 % dataLength; uint32_t xIndex2 = ((uint32_t)key[170] + 500021189u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 46:{ uint32_t xIndex = 3265965994 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[197] - 116u); }break;
			case 47:{ uint32_t xIndex = 500021189 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[116] - 103u); }break;
			case 48:{ uint32_t xIndex = 1927072628 % dataLength; uint32_t shift = (uint32_t)(key[103] + 3032601998) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 49:{ uint32_t xIndex = 2952203367 % dataLength; uint32_t shift = (uint32_t)(key[142] + 3745830425) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 50:{ uint32_t xIndex1 = 3032601998 % dataLength; uint32_t xIndex2 = ((uint32_t)key[25] + 763478136u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 51:{ uint32_t xIndex1 = 3745830425 % dataLength; uint32_t xIndex2 = ((uint32_t)key[120] + 808009051u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 52:{ uint32_t xIndex1 = 763478136 % dataLength; uint32_t xIndex2 = ((uint32_t)key[91] + 470291122u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 53:{ uint32_t xIndex = 808009051 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[178] ^ 173u); }break;
			case 54:{ uint32_t xIndex = 470291122 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[173] - 188u); }break;
			case 55:{ uint32_t xIndex = 152809133 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[188] - 143u); }break;
			case 56:{ uint32_t xIndex1 = 1262519996 % dataLength; uint32_t xIndex2 = ((uint32_t)key[143] + 3073944342u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 57:{ uint32_t xIndex1 = 3596810895 % dataLength; uint32_t xIndex2 = ((uint32_t)key[22] + 3912267137u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 58:{ uint32_t xIndex = 3073944342 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[129] - 64u); }break;
			case 59:{ uint32_t xIndex = 3912267137 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[64] ^ 3u); }break;
			case 60:{ uint32_t xIndex1 = 2219737664 % dataLength; uint32_t xIndex2 = ((uint32_t)key[3] + 550773434u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 61:{ uint32_t xIndex1 = 3566475267 % dataLength; uint32_t xIndex2 = ((uint32_t)key[186] + 1594990229u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 62:{ uint32_t xIndex = 550773434 % dataLength; uint32_t shift = (uint32_t)(key[149] + 1825593092) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 63:{ uint32_t xIndex1 = 1594990229 % dataLength; uint32_t xIndex2 = ((uint32_t)key[4] + 366913975u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 64:{ uint32_t xIndex = 1825593092 % dataLength; uint32_t shift = (uint32_t)(key[183] + 3770957214) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 65:{ uint32_t xIndex = 366913975 % dataLength; uint32_t shift = (uint32_t)(key[158] + 923917801) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 66:{ uint32_t xIndex1 = 3770957214 % dataLength; uint32_t xIndex2 = ((uint32_t)key[233] + 3013470472u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 67:{ uint32_t xIndex = 923917801 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[8] ^ 171u); }break;
			case 68:{ uint32_t xIndex = 3013470472 % dataLength; uint32_t shift = (uint32_t)(key[171] + 1094713282) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 69:{ uint32_t xIndex = 2454389675 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[194] - 125u); }break;
			case 70:{ uint32_t xIndex1 = 1094713282 % dataLength; uint32_t xIndex2 = ((uint32_t)key[125] + 1044798540u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 71:{ uint32_t xIndex = 1000089469 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[76] ^ 223u); }break;
			case 72:{ uint32_t xIndex1 = 1044798540 % dataLength; uint32_t xIndex2 = ((uint32_t)key[223] + 3941575974u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 73:{ uint32_t xIndex = 180074975 % dataLength; uint32_t shift = (uint32_t)(key[38] + 4039618385) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 74:{ uint32_t xIndex = 3941575974 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[81] ^ 208u); }break;
			case 75:{ uint32_t xIndex = 4039618385 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[208] ^ 83u); }break;
			case 76:{ uint32_t xIndex = 1179915472 % dataLength; uint32_t shift = (uint32_t)(key[83] + 2354104778) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 77:{ uint32_t xIndex = 3240206419 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[202] ^ 101u); }break;
			case 78:{ uint32_t xIndex1 = 2354104778 % dataLength; uint32_t xIndex2 = ((uint32_t)key[101] + 3687888532u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 79:{ uint32_t xIndex = 774629733 % dataLength; uint32_t shift = (uint32_t)(key[148] + 15715079) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 80:{ uint32_t xIndex1 = 3687888532 % dataLength; uint32_t xIndex2 = ((uint32_t)key[7] + 274340270u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 81:{ uint32_t xIndex1 = 15715079 % dataLength; uint32_t xIndex2 = ((uint32_t)key[174] + 183798201u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 82:{ uint32_t xIndex1 = 274340270 % dataLength; uint32_t xIndex2 = ((uint32_t)key[185] + 1896424856u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 83:{ uint32_t xIndex = 183798201 % dataLength; uint32_t shift = (uint32_t)(key[152] + 2345635323) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 84:{ uint32_t xIndex = 1896424856 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[251] ^ 210u); }break;
			case 85:{ uint32_t xIndex = 2345635323 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[210] - 77u); }break;
			case 86:{ uint32_t xIndex = 677355730 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[77] - 220u); }break;
			case 87:{ uint32_t xIndex1 = 3493168205 % dataLength; uint32_t xIndex2 = ((uint32_t)key[220] + 1049918767u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 88:{ uint32_t xIndex = 1402045916 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[47] ^ 54u); }break;
			case 89:{ uint32_t xIndex = 1049918767 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[54] ^ 33u); }break;
			case 90:{ uint32_t xIndex = 758555446 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[33] - 96u); }break;
			case 91:{ uint32_t xIndex1 = 3850308897 % dataLength; uint32_t xIndex2 = ((uint32_t)key[96] + 2188405923u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 92:{ uint32_t xIndex = 914963296 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[163] - 218u); }break;
			case 93:{ uint32_t xIndex1 = 2188405923 % dataLength; uint32_t xIndex2 = ((uint32_t)key[218] + 3954284597u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 94:{ uint32_t xIndex = 2610537690 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[53] - 36u); }break;
			case 95:{ uint32_t xIndex1 = 3954284597 % dataLength; uint32_t xIndex2 = ((uint32_t)key[36] + 1583837271u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 96:{ uint32_t xIndex = 840886820 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[87] ^ 190u); }break;
			case 97:{ uint32_t xIndex1 = 1583837271 % dataLength; uint32_t xIndex2 = ((uint32_t)key[190] + 2047990153u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 98:{ uint32_t xIndex = 2949409214 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[137] ^ 40u); }break;
			case 99:{ uint32_t xIndex = 2047990153 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[40] - 75u); }break;
			case 100:{ uint32_t xIndex = 3696522792 % dataLength; uint32_t shift = (uint32_t)(key[75] + 3833591266) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 101:{ uint32_t xIndex1 = 1023663179 % dataLength; uint32_t xIndex2 = ((uint32_t)key[226] + 832357661u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 102:{ uint32_t xIndex1 = 3833591266 % dataLength; uint32_t xIndex2 = ((uint32_t)key[29] + 1594033004u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 103:{ uint32_t xIndex = 832357661 % dataLength; uint32_t shift = (uint32_t)(key[108] + 3125429375) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 104:{ uint32_t xIndex = 1594033004 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[127] - 70u); }break;
			case 105:{ uint32_t xIndex = 3125429375 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[70] - 241u); }break;
			case 106:{ uint32_t xIndex = 1492553030 % dataLength; uint32_t shift = (uint32_t)(key[241] + 2938041840) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 107:{ uint32_t xIndex = 446402289 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[240] ^ 243u); }break;
			case 108:{ uint32_t xIndex = 2938041840 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[243] ^ 234u); }break;
			case 109:{ uint32_t xIndex = 233667827 % dataLength; uint32_t shift = (uint32_t)(key[234] + 2155326213) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 110:{ uint32_t xIndex = 1650963434 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[5] ^ 180u); }break;
			case 111:{ uint32_t xIndex1 = 2155326213 % dataLength; uint32_t xIndex2 = ((uint32_t)key[180] + 3854735783u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 112:{ uint32_t xIndex = 1228665268 % dataLength; uint32_t shift = (uint32_t)(key[167] + 3433311694) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 113:{ uint32_t xIndex1 = 3854735783 % dataLength; uint32_t xIndex2 = ((uint32_t)key[206] + 3218001241u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 114:{ uint32_t xIndex1 = 3433311694 % dataLength; uint32_t xIndex2 = ((uint32_t)key[89] + 391174840u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 115:{ uint32_t xIndex1 = 3218001241 % dataLength; uint32_t xIndex2 = ((uint32_t)key[184] + 3295997595u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 116:{ uint32_t xIndex = 391174840 % dataLength; uint32_t shift = (uint32_t)(key[155] + 2381971186) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 117:{ uint32_t xIndex = 3295997595 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[242] - 237u); }break;
			case 118:{ uint32_t xIndex = 2381971186 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[237] ^ 252u); }break;
			case 119:{ uint32_t xIndex = 3133598189 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[252] ^ 207u); }break;
			case 120:{ uint32_t xIndex = 1673254140 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[207] - 86u); }break;
			case 121:{ uint32_t xIndex = 947523535 % dataLength; uint32_t shift = (uint32_t)(key[86] + 3886167233) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 122:{ uint32_t xIndex = 4065880918 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[193] ^ 128u); }break;
			case 123:{ uint32_t xIndex = 3886167233 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[128] ^ 67u); }break;
			case 124:{ uint32_t xIndex1 = 4253435008 % dataLength; uint32_t xIndex2 = ((uint32_t)key[67] + 2176055034u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 125:{ uint32_t xIndex1 = 2135281987 % dataLength; uint32_t xIndex2 = ((uint32_t)key[250] + 764617173u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 126:{ uint32_t xIndex1 = 2176055034 % dataLength; uint32_t xIndex2 = ((uint32_t)key[213] + 4058565956u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 127:{ uint32_t xIndex1 = 764617173 % dataLength; uint32_t xIndex2 = ((uint32_t)key[68] + 1757300471u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 128:{ uint32_t xIndex = 4058565956 % dataLength; uint32_t shift = (uint32_t)(key[247] + 631924190) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 129:{ uint32_t xIndex1 = 1757300471 % dataLength; uint32_t xIndex2 = ((uint32_t)key[222] + 63989033u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 130:{ uint32_t xIndex = 631924190 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[41] - 72u); }break;
			case 131:{ uint32_t xIndex = 63989033 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[72] - 235u); }break;
			case 132:{ uint32_t xIndex = 2131710792 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[235] ^ 2u); }break;
			case 133:{ uint32_t xIndex1 = 2398078187 % dataLength; uint32_t xIndex2 = ((uint32_t)key[2] + 2263413437u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 134:{ uint32_t xIndex1 = 1382464514 % dataLength; uint32_t xIndex2 = ((uint32_t)key[189] + 605878924u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 135:{ uint32_t xIndex1 = 2263413437 % dataLength; uint32_t xIndex2 = ((uint32_t)key[140] + 906026783u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 136:{ uint32_t xIndex = 605878924 % dataLength; uint32_t shift = (uint32_t)(key[31] + 918895974) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 137:{ uint32_t xIndex1 = 906026783 % dataLength; uint32_t xIndex2 = ((uint32_t)key[102] + 2019032721u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 138:{ uint32_t xIndex = 918895974 % dataLength; uint32_t shift = (uint32_t)(key[145] + 4067436304) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 139:{ uint32_t xIndex1 = 2019032721 % dataLength; uint32_t xIndex2 = ((uint32_t)key[16] + 1751542163u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 140:{ uint32_t xIndex = 4067436304 % dataLength; uint32_t shift = (uint32_t)(key[147] + 934768138) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 141:{ uint32_t xIndex = 1751542163 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[10] - 165u); }break;
			case 142:{ uint32_t xIndex = 934768138 % dataLength; uint32_t shift = (uint32_t)(key[165] + 2217114836) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 143:{ uint32_t xIndex1 = 1549335717 % dataLength; uint32_t xIndex2 = ((uint32_t)key[212] + 592934983u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 144:{ uint32_t xIndex1 = 2217114836 % dataLength; uint32_t xIndex2 = ((uint32_t)key[71] + 988287470u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 145:{ uint32_t xIndex1 = 592934983 % dataLength; uint32_t xIndex2 = ((uint32_t)key[238] + 704356601u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 146:{ uint32_t xIndex = 988287470 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[249] - 216u); }break;
			case 147:{ uint32_t xIndex1 = 704356601 % dataLength; uint32_t xIndex2 = ((uint32_t)key[216] + 2810273595u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 148:{ uint32_t xIndex = 1004593112 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[59] ^ 18u); }break;
			case 149:{ uint32_t xIndex = 2810273595 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[18] - 141u); }break;
			case 150:{ uint32_t xIndex1 = 2225024274 % dataLength; uint32_t xIndex2 = ((uint32_t)key[141] + 2982638620u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 151:{ uint32_t xIndex = 1098375053 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[28] ^ 111u); }break;
			case 152:{ uint32_t xIndex = 2982638620 % dataLength; uint32_t shift = (uint32_t)(key[111] + 753271670) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 153:{ uint32_t xIndex1 = 1107014255 % dataLength; uint32_t xIndex2 = ((uint32_t)key[118] + 2629954657u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 154:{ uint32_t xIndex = 753271670 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[97] ^ 160u); }break;
			case 155:{ uint32_t xIndex = 2629954657 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[160] ^ 227u); }break;
			case 156:{ uint32_t xIndex = 1909300640 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[227] ^ 26u); }break;
			case 157:{ uint32_t xIndex1 = 561792483 % dataLength; uint32_t xIndex2 = ((uint32_t)key[26] + 1851669365u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 158:{ uint32_t xIndex = 1877678362 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[117] ^ 100u); }break;
			case 159:{ uint32_t xIndex1 = 1851669365 % dataLength; uint32_t xIndex2 = ((uint32_t)key[100] + 197864855u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 160:{ uint32_t xIndex1 = 1865810020 % dataLength; uint32_t xIndex2 = ((uint32_t)key[151] + 1571172862u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 161:{ uint32_t xIndex = 197864855 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[254] ^ 201u); }break;
			case 162:{ uint32_t xIndex = 1571172862 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[201] ^ 104u); }break;
			case 163:{ uint32_t xIndex = 2725609673 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[104] ^ 139u); }break;
			case 164:{ uint32_t xIndex1 = 3512107112 % dataLength; uint32_t xIndex2 = ((uint32_t)key[139] + 2898022946u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 165:{ uint32_t xIndex1 = 125222283 % dataLength; uint32_t xIndex2 = ((uint32_t)key[34] + 4129861725u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 166:{ uint32_t xIndex1 = 2898022946 % dataLength; uint32_t xIndex2 = ((uint32_t)key[93] + 4254876076u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 167:{ uint32_t xIndex1 = 4129861725 % dataLength; uint32_t xIndex2 = ((uint32_t)key[172] + 1304922559u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 168:{ uint32_t xIndex = 4254876076 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[191] - 134u); }break;
			case 169:{ uint32_t xIndex = 1304922559 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[134] ^ 49u); }break;
			case 170:{ uint32_t xIndex = 3030629766 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[49] - 48u); }break;
			case 171:{ uint32_t xIndex = 2569338417 % dataLength; uint32_t shift = (uint32_t)(key[48] + 418670131) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 172:{ uint32_t xIndex = 47163440 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[51] - 42u); }break;
			case 173:{ uint32_t xIndex1 = 418670131 % dataLength; uint32_t xIndex2 = ((uint32_t)key[42] + 2373443141u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 174:{ uint32_t xIndex = 3540514858 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[69] ^ 244u); }break;
			case 175:{ uint32_t xIndex = 2373443141 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[244] - 231u); }break;
			case 176:{ uint32_t xIndex1 = 3382204404 % dataLength; uint32_t xIndex2 = ((uint32_t)key[231] + 3228419598u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 177:{ uint32_t xIndex = 580282087 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[14] - 153u); }break;
			case 178:{ uint32_t xIndex = 3228419598 % dataLength; uint32_t shift = (uint32_t)(key[153] + 2923508984) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 179:{ uint32_t xIndex1 = 966984857 % dataLength; uint32_t xIndex2 = ((uint32_t)key[248] + 3864846299u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 180:{ uint32_t xIndex1 = 2923508984 % dataLength; uint32_t xIndex2 = ((uint32_t)key[219] + 2551655218u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 181:{ uint32_t xIndex1 = 3864846299 % dataLength; uint32_t xIndex2 = ((uint32_t)key[50] + 3773850925u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 182:{ uint32_t xIndex = 2551655218 % dataLength; uint32_t shift = (uint32_t)(key[45] + 1740399420) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 183:{ uint32_t xIndex = 3773850925 % dataLength; uint32_t shift = (uint32_t)(key[60] + 4244727055) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 184:{ uint32_t xIndex1 = 1740399420 % dataLength; uint32_t xIndex2 = ((uint32_t)key[15] + 2535943062u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 185:{ uint32_t xIndex = 4244727055 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[150] ^ 1u); }break;
			case 186:{ uint32_t xIndex = 2535943062 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[1] ^ 192u); }break;
			case 187:{ uint32_t xIndex = 4127601665 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[192] ^ 131u); }break;
			case 188:{ uint32_t xIndex1 = 1608992448 % dataLength; uint32_t xIndex2 = ((uint32_t)key[131] + 3607563066u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 189:{ uint32_t xIndex = 595315331 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[58] ^ 21u); }break;
			case 190:{ uint32_t xIndex1 = 3607563066 % dataLength; uint32_t xIndex2 = ((uint32_t)key[21] + 3775824772u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 191:{ uint32_t xIndex = 2075845909 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[132] - 55u); }break;
			case 192:{ uint32_t xIndex = 3775824772 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[55] - 30u); }break;
			case 193:{ uint32_t xIndex = 3262522423 % dataLength; uint32_t shift = (uint32_t)(key[30] + 3895012457) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 194:{ uint32_t xIndex1 = 2265435678 % dataLength; uint32_t xIndex2 = ((uint32_t)key[105] + 3165781384u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 195:{ uint32_t xIndex1 = 3895012457 % dataLength; uint32_t xIndex2 = ((uint32_t)key[136] + 167822891u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 196:{ uint32_t xIndex1 = 3165781384 % dataLength; uint32_t xIndex2 = ((uint32_t)key[43] + 1766373442u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 197:{ uint32_t xIndex1 = 167822891 % dataLength; uint32_t xIndex2 = ((uint32_t)key[66] + 1040449021u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 198:{ uint32_t xIndex = 1766373442 % dataLength; uint32_t shift = (uint32_t)(key[253] + 1334368460) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 199:{ uint32_t xIndex = 1040449021 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[204] - 95u); }break;
			case 200:{ uint32_t xIndex1 = 1334368460 % dataLength; uint32_t xIndex2 = ((uint32_t)key[95] + 2530872742u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 201:{ uint32_t xIndex = 4119217247 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[166] - 209u); }break;
			case 202:{ uint32_t xIndex = 2530872742 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[209] ^ 80u); }break;
			case 203:{ uint32_t xIndex1 = 1344966097 % dataLength; uint32_t xIndex2 = ((uint32_t)key[80] + 3422515923u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 204:{ uint32_t xIndex1 = 113604944 % dataLength; uint32_t xIndex2 = ((uint32_t)key[211] + 3475067466u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 205:{ uint32_t xIndex1 = 3422515923 % dataLength; uint32_t xIndex2 = ((uint32_t)key[74] + 1669091301u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 206:{ uint32_t xIndex = 3475067466 % dataLength; uint32_t shift = (uint32_t)(key[229] + 3399058196) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 207:{ uint32_t xIndex1 = 1669091301 % dataLength; uint32_t xIndex2 = ((uint32_t)key[20] + 4033308039u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 208:{ uint32_t xIndex = 3399058196 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[135] - 46u); }break;
			case 209:{ uint32_t xIndex = 4033308039 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[46] - 57u); }break;
			case 210:{ uint32_t xIndex = 3598535214 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[57] - 24u); }break;
			case 211:{ uint32_t xIndex = 2733439033 % dataLength; uint32_t shift = (uint32_t)(key[24] + 376402043) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 212:{ uint32_t xIndex = 4059683352 % dataLength; uint32_t shift = (uint32_t)(key[123] + 2821323090) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 213:{ uint32_t xIndex1 = 376402043 % dataLength; uint32_t xIndex2 = ((uint32_t)key[82] + 433617613u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 214:{ uint32_t xIndex1 = 2821323090 % dataLength; uint32_t xIndex2 = ((uint32_t)key[205] + 2745344604u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 215:{ uint32_t xIndex1 = 433617613 % dataLength; uint32_t xIndex2 = ((uint32_t)key[92] + 796075951u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 216:{ uint32_t xIndex = 2745344604 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[175] ^ 182u); }break;
			case 217:{ uint32_t xIndex1 = 796075951 % dataLength; uint32_t xIndex2 = ((uint32_t)key[182] + 680987553u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 218:{ uint32_t xIndex = 2137301942 % dataLength; uint32_t shift = (uint32_t)(key[161] + 3361423328) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 219:{ uint32_t xIndex = 680987553 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[224] - 35u); }break;
			case 220:{ uint32_t xIndex1 = 3361423328 % dataLength; uint32_t xIndex2 = ((uint32_t)key[35] + 4224699738u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 221:{ uint32_t xIndex1 = 2745982755 % dataLength; uint32_t xIndex2 = ((uint32_t)key[90] + 2807111349u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 222:{ uint32_t xIndex1 = 4224699738 % dataLength; uint32_t xIndex2 = ((uint32_t)key[181] + 4068103844u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 223:{ uint32_t xIndex = 2807111349 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[164] ^ 215u); }break;
			case 224:{ uint32_t xIndex1 = 4068103844 % dataLength; uint32_t xIndex2 = ((uint32_t)key[215] + 3843504702u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 225:{ uint32_t xIndex = 2879859415 % dataLength; uint32_t shift = (uint32_t)(key[62] + 722692105) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 226:{ uint32_t xIndex = 3843504702 % dataLength; uint32_t shift = (uint32_t)(key[9] + 3735669416) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 227:{ uint32_t xIndex = 722692105 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[168] ^ 203u); }break;
			case 228:{ uint32_t xIndex1 = 3735669416 % dataLength; uint32_t xIndex2 = ((uint32_t)key[203] + 1372843618u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 229:{ uint32_t xIndex = 3723878091 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[98] ^ 157u); }break;
			case 230:{ uint32_t xIndex = 1372843618 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[157] - 236u); }break;
			case 231:{ uint32_t xIndex = 555932573 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[236] - 255u); }break;
			case 232:{ uint32_t xIndex1 = 1911209964 % dataLength; uint32_t xIndex2 = ((uint32_t)key[255] + 900738502u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 233:{ uint32_t xIndex1 = 1160056575 % dataLength; uint32_t xIndex2 = ((uint32_t)key[198] + 3029380465u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 234:{ uint32_t xIndex = 900738502 % dataLength; uint32_t shift = (uint32_t)(key[113] + 1490655856) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 235:{ uint32_t xIndex = 3029380465 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[112] ^ 115u); }break;
			case 236:{ uint32_t xIndex1 = 1490655856 % dataLength; uint32_t xIndex2 = ((uint32_t)key[115] + 2597026922u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 237:{ uint32_t xIndex = 2448396147 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[106] ^ 133u); }break;
			case 238:{ uint32_t xIndex = 2597026922 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[133] - 52u); }break;
			case 239:{ uint32_t xIndex1 = 2987282821 % dataLength; uint32_t xIndex2 = ((uint32_t)key[52] + 1035105319u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 240:{ uint32_t xIndex1 = 2888957492 % dataLength; uint32_t xIndex2 = ((uint32_t)key[39] + 173973070u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 241:{ uint32_t xIndex = 1035105319 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[78] - 217u); }break;
			case 242:{ uint32_t xIndex1 = 173973070 % dataLength; uint32_t xIndex2 = ((uint32_t)key[217] + 1049808696u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 243:{ uint32_t xIndex = 3724639193 % dataLength; uint32_t shift = (uint32_t)(key[56] + 2971734299) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 244:{ uint32_t xIndex = 1049808696 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[27] ^ 114u); }break;
			case 245:{ uint32_t xIndex1 = 2971734299 % dataLength; uint32_t xIndex2 = ((uint32_t)key[114] + 1893212269u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 246:{ uint32_t xIndex = 3902773106 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[109] - 124u); }break;
			case 247:{ uint32_t xIndex = 1893212269 % dataLength; uint32_t shift = (uint32_t)(key[124] + 3275291215) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 248:{ uint32_t xIndex = 2005021052 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] - (uint32_t)key[79] - 214u); }break;
			case 249:{ uint32_t xIndex = 3275291215 % dataLength; uint32_t shift = (uint32_t)(key[214] + 2912711489) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 250:{ uint32_t xIndex1 = 3353717718 % dataLength; uint32_t xIndex2 = ((uint32_t)key[65] + 3752954112u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 251:{ uint32_t xIndex = 2912711489 % dataLength; data[xIndex] = (byte)((uint32_t)data[xIndex] ^ (uint32_t)key[0] ^ 195u); }break;
			case 252:{ uint32_t xIndex1 = 3752954112 % dataLength; uint32_t xIndex2 = ((uint32_t)key[195] + 4144848762u) % dataLength; byte a = data[xIndex1]; byte b = data[xIndex2] - 1; data[xIndex1] = a - b; data[xIndex2] = b; }break;
			case 253:{ uint32_t xIndex = 611713987 % dataLength; uint32_t shift = (uint32_t)(key[122] + 3335055445) % 8; data[xIndex] = (byte)(((uint32_t)data[xIndex] >> shift) | ((uint32_t)data[xIndex] << (8 - shift))); }break;
			case 254:{ uint32_t xIndex1 = 4144848762 % dataLength; uint32_t xIndex2 = ((uint32_t)key[85] + 3263265220u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;
			case 255:{ uint32_t xIndex1 = 3335055445 % dataLength; uint32_t xIndex2 = ((uint32_t)key[196] + 2319860087u) % dataLength; byte temp = data[xIndex1]; data[xIndex1] = data[xIndex2]; data[xIndex2] = temp; }break;


				//!!!}}INSTRUCTIONS
			}
		}
	}

}
}

