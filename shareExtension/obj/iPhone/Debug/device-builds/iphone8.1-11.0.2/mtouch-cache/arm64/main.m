#include "xamarin/xamarin.h"

extern void *mono_aot_module_shareExtension_info;
extern void *mono_aot_module_mscorlib_info;
extern void *mono_aot_module_Xamarin_iOS_info;

void xamarin_register_modules_impl ()
{
	mono_aot_register_module (mono_aot_module_shareExtension_info);
	mono_aot_register_module (mono_aot_module_mscorlib_info);
	mono_aot_register_module (mono_aot_module_Xamarin_iOS_info);

}

void xamarin_register_assemblies_impl ()
{
	guint32 exception_gchandle = 0;

}

extern "C" void xamarin_create_classes();
void xamarin_setup_impl ()
{
	xamarin_create_classes();
	xamarin_gc_pump = FALSE;
	xamarin_init_mono_debug = TRUE;
	xamarin_executable_name = "shareExtension.dll";
	mono_use_llvm = FALSE;
	xamarin_log_level = 4;
	xamarin_arch_name = "arm64";
	xamarin_marshal_objectivec_exception_mode = MarshalObjectiveCExceptionModeDisable;
	xamarin_debug_mode = TRUE;
	setenv ("MONO_GC_PARAMS", "nursery-size=512k,major=marksweep", 1);
}

int main (int argc, char **argv)
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	argv [0] = (char *) "shareExtension";
	int rv = xamarin_main (argc, argv, XamarinLaunchModeExtension);
	[pool drain];
	return rv;
}
void xamarin_initialize_callbacks () __attribute__ ((constructor));
void xamarin_initialize_callbacks ()
{
	xamarin_setup = xamarin_setup_impl;
	xamarin_register_assemblies = xamarin_register_assemblies_impl;
	xamarin_register_modules = xamarin_register_modules_impl;
}
