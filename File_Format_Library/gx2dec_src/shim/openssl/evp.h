#pragma once
// minimal EVP stub: FetchShader's pipeline-hash digest (value unused by standalone decompile)
#include <cstring>
struct EVP_MD_CTX_dummy { int _; };
typedef EVP_MD_CTX_dummy EVP_MD_CTX;
typedef void EVP_MD;
inline EVP_MD_CTX* EVP_MD_CTX_new() { static EVP_MD_CTX_dummy d; return &d; }
inline void EVP_MD_CTX_free(EVP_MD_CTX*) {}
inline const EVP_MD* EVP_sha1() { return nullptr; }
inline int EVP_DigestInit(EVP_MD_CTX*, const EVP_MD*) { return 1; }
inline int EVP_DigestUpdate(EVP_MD_CTX*, const void*, size_t) { return 1; }
inline int EVP_DigestFinal_ex(EVP_MD_CTX*, unsigned char* md, unsigned int* len) { if (md) memset(md, 0, 20); if (len) *len = 20; return 1; }
