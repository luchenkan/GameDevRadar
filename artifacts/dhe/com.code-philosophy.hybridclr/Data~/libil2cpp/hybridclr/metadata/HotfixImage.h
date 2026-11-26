#pragma once

#include <vector>
#include <unordered_set>

#include "SuperSetAOTHomologousImage.h"
#include "utils/Il2CppHashMap.h"
#include "utils/HashUtils.h"

namespace hybridclr
{
	namespace metadata
	{


		class HotfixImage : public SuperSetAOTHomologousImage
		{
		public:

			static HotfixImage* GetHotfixAssemblyByTargetAssembly(const Il2CppAssembly* ass);
			static void RegisteHotfixImage(const Il2CppAssembly* targetAssembly, HotfixImage* hotfixImage);
			static void ResetMethod(const MethodInfo* targetMethod);

			HotfixImage(std::vector<uint32_t>& methodTokens) : SuperSetAOTHomologousImage(), _fixedMethodTokens(methodTokens.begin(), methodTokens.end()) {}

			bool NeedHotfixed(uint32_t methodToken) const
			{
				return _fixedMethodTokens.find(methodToken) != _fixedMethodTokens.end();
			}

			bool NeedAccessMethodBody(const Il2CppTypeDefinition* targetTypeDef, const Il2CppMethodDefinition* targetMethodDef) override
			{
				return _fixedMethodTokens.find(targetMethodDef->token) != _fixedMethodTokens.end();
			}

			void HotfixMethods();

			Il2CppGenericContainer* GetGenericContainerByRawIndex(uint32_t index) override;

		private:
			std::unordered_set<uint32_t> _fixedMethodTokens;
		};
	}
}