/*
	Copyright(c) 2015 Neodymium

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/

#pragma managed

#include <vcclr.h>
#include "DirectXTex.h"
#define _WIN32_WINNT 0x0600 

using namespace System;
using namespace System::IO;
using namespace System::Runtime::InteropServices;

namespace DirectXTex
{
	public ref class ImageCompressor
	{
	public:

		static array<Byte>^ Decompress(array<Byte>^ data, int width, int height, int format)
		{
			size_t rowPitch;
			size_t slicePitch;
			DirectX::ComputePitch((DXGI_FORMAT)format, width, height, rowPitch, slicePitch);

			if (data->Length == slicePitch)
			{
				uint8_t *buf = new uint8_t[slicePitch];
				Marshal::Copy(data, 0, (IntPtr)buf, slicePitch);
				
				DirectX::Image inputImage;
				inputImage.width = width;
				inputImage.height = height;
				inputImage.format = (DXGI_FORMAT)format;
				inputImage.pixels = buf;
				inputImage.rowPitch = rowPitch;
				inputImage.slicePitch = slicePitch;

				DirectX::ScratchImage outputImage;
				

				// decompress image
				DirectX::Decompress(inputImage, DXGI_FORMAT_R8G8B8A8_UNORM, outputImage);

				array<Byte>^ result = gcnew array<Byte>(4 * width * height);
				Marshal::Copy((IntPtr)outputImage.GetPixels(), result, 0, 4 * width * height);

				delete[] buf;
				return result;
			}
			else
			{
				throw gcnew Exception("Compressed image should be " + slicePitch.ToString() + " bytes");
			}			
		}

		static array<Byte>^ Compress(array<Byte>^ data, int width, int height, int format)
		{
			size_t rowPitch = width * 4;
			size_t slicePitch = width * height * 4;
			if (data->Length == slicePitch)
			{
				uint8_t *buf = new uint8_t[slicePitch];
				Marshal::Copy(data, 0, (IntPtr)buf, slicePitch);

				DirectX::Image inputImage;
				inputImage.width = width;
				inputImage.height = height;
				inputImage.format = DXGI_FORMAT_R8G8B8A8_UNORM;
				inputImage.pixels = buf;
				inputImage.rowPitch = rowPitch;
				inputImage.slicePitch = slicePitch;

				DirectX::ScratchImage outputImage;

				// compress image
				DirectX::Compress(inputImage, (DXGI_FORMAT)format, 0, 1, outputImage);

				size_t rowPitchOut;
				size_t slicePitchOut;
				DirectX::ComputePitch((DXGI_FORMAT)format, width, height, rowPitchOut, slicePitchOut);
				array<Byte>^ result = gcnew array<Byte>(slicePitchOut);
				Marshal::Copy((IntPtr)outputImage.GetPixels(), result, 0, slicePitchOut);

				delete[] buf;
				return result;
			}
			else
			{
				throw gcnew Exception("Uncompressed image should be " + slicePitch.ToString() + " bytes");
			}			
		}
	};

	public ref class ImageConverter
	{
	public:

		static array<Byte>^ Convert(array<Byte>^ data, int width, int height, int inputFormat, int outputFormat)
		{
			size_t inputRowPitch;
			size_t inrputSlicePitch;
			DirectX::ComputePitch((DXGI_FORMAT)inputFormat, width, height, inputRowPitch, inrputSlicePitch);

			if (data->Length == inrputSlicePitch)
			{
				uint8_t *buf = new uint8_t[inrputSlicePitch];
				Marshal::Copy(data, 0, (IntPtr)buf, inrputSlicePitch);

				DirectX::Image inputImage;
				inputImage.width = width;
				inputImage.height = height;
				inputImage.format = (DXGI_FORMAT)inputFormat;
				inputImage.pixels = buf;
				inputImage.rowPitch = inputRowPitch;
				inputImage.slicePitch = inrputSlicePitch;

				DirectX::ScratchImage outputImage;

				// convert image
				DirectX::Convert(inputImage, (DXGI_FORMAT)outputFormat, 0, 0, outputImage);

				size_t outputRowPitch;
				size_t outputSlicePitch;
				DirectX::ComputePitch((DXGI_FORMAT)outputFormat, width, height, outputRowPitch, outputSlicePitch);
				array<Byte>^ result = gcnew array<Byte>(outputSlicePitch);
				Marshal::Copy((IntPtr)outputImage.GetPixels(), result, 0, outputSlicePitch);

				delete[] buf;
				return result;
			}
			else
			{
				throw gcnew Exception("Input image should be " + inrputSlicePitch.ToString() + " bytes");
			}
		}
	};
	
	public ref class ImageStruct
	{
	public:
		property int Width;
		property int Height;
		//property int Stride;
		property int Format;
		property int MipMapLevels;
		property array<Byte>^ Data;

		int GetRowPitch()
		{
			size_t rowPitch;
			size_t slicePitch;
			DirectX::ComputePitch((DXGI_FORMAT)Format, Width, Height, rowPitch, slicePitch);
			return rowPitch;
		}

		int GetSlicePitch()
		{
			size_t rowPitch;
			size_t slicePitch;
			DirectX::ComputePitch((DXGI_FORMAT)Format, Width, Height, rowPitch, slicePitch);
			return slicePitch;
		}
	};

	public ref class DDSIO
	{
	private:
	public:
		
		static ImageStruct^ ReadDDS(String^ fileName)
		{		
			DirectX::TexMetadata meta;
			DirectX::ScratchImage im;

			pin_ptr<const wchar_t> wname = PtrToStringChars(fileName);

			// load dds
			HRESULT x = DirectX::LoadFromDDSFile(wname, 0, &meta, im);

			ImageStruct^ result = gcnew ImageStruct();

			result->MipMapLevels = im.GetImageCount();
			result->Width = im.GetImage(0, 0, 0)->width;
			result->Height = im.GetImage(0, 0, 0)->height;
//			result->Stride = im.GetImage(0, 0, 0)->rowPitch;
			result->Format = im.GetImage(0, 0, 0)->format;
			result->Data = gcnew array<Byte>(im.GetPixelsSize());
			Marshal::Copy((IntPtr)im.GetPixels(), result->Data, 0, im.GetPixelsSize());

			return result;
		}

		//static ImageStruct^ ReadDDS(Stream^ stream)
		//{
		//	DirectX::TexMetadata meta;
		//	DirectX::ScratchImage im;

		//	//pin_ptr<const wchar_t> wname = PtrToStringChars(fileName);
		//	HRESULT x = DirectX:::LoadFromDDSMemory(

		//	ImageStruct^ result = gcnew ImageStruct();

		//	result->mipMapLevels = im.GetImageCount();
		//	result->width = im.GetImage(0, 0, 0)->width;
		//	result->height = im.GetImage(0, 0, 0)->height;
		//	result->stride = im.GetImage(0, 0, 0)->rowPitch;
		//	result->format = im.GetImage(0, 0, 0)->format;
		//	result->data = gcnew array<Byte>(im.GetPixelsSize());
		//	Marshal::Copy((IntPtr)im.GetPixels(), result->data, 0, im.GetPixelsSize());

		//	return result;
		//}

		static void WriteDDS(String^ fileName, ImageStruct^ image)
		{
			uint8_t *buf = new uint8_t[image->Data->Length];
			Marshal::Copy(image->Data, 0, (IntPtr)buf, image->Data->Length);
			
			DirectX::TexMetadata meta;
			meta.width = image->Width;
			meta.height = image->Height;
			meta.depth = 1;
			meta.arraySize = 1; // ???
			meta.mipLevels = image->MipMapLevels;
			meta.miscFlags = 0; // ???
			meta.miscFlags2 = 0; // ???
			meta.format = (DXGI_FORMAT)image->Format;
			meta.dimension = DirectX::TEX_DIMENSION_TEXTURE2D;

			DirectX::Image *images = new DirectX::Image[image->MipMapLevels];

			int div = 1;
			int add = 0;
			for (int i = 0; i < image->MipMapLevels; i++)
			{
				images[i].width = image->Width / div;
				images[i].height = image->Height / div;
				images[i].format = (DXGI_FORMAT)image->Format;
				images[i].pixels = buf + add;

				DirectX::ComputePitch(images[i].format, images[i].width, images[i].height, images[i].rowPitch, images[i].slicePitch, 0);

				add += images[i].slicePitch;
				div *= 2;
			}

			pin_ptr<const wchar_t> wname = PtrToStringChars(fileName);

			// save dds
			DirectX::SaveToDDSFile(images, image->MipMapLevels, meta, 0, wname);

			delete[] images;
		}
	};
}

