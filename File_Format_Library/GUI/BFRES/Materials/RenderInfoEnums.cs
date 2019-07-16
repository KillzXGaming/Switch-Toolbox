using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstPlugin
{
    public class RenderInfoEnums
    {
        public class SMO //Super Mario Odyssey
        {
            public enum map_category : int { None, Building, Ground, Road, Water, Lava, Rock, Other, DamageArea, }
            public enum proc_texture_2d_type : int { PerlinFbm, Worley, }
            public enum proc_texture_3d_type : int { Caustics, Ridge, CloudLikeFbm, }
            public enum display_face : int { front, both, back, none, }
            public enum forward_xlu : int { Blend, Opa, Mul, Add, }
            public enum color_blend_rgb_op : int { add, src_minus_dst, }
            public enum color_blend_rgb_src_func : int { src_alpha, one, }
            public enum color_blend_rgb_dst_func : int { one_minus_src_alpha, zero, one, }
            public enum color_blend_alpha_op : int { add, }
            public enum color_blend_alpha_src_func : int { one, zero, src_alpha, }
            public enum color_blend_alpha_dst_func : int { zero, one, }
            public enum deferred_xlu : int { BcNrmLbuf, BcLbuf, FootPrint, BcLbufAdd, LbufAdd, LbufMul, BcNrmLbufRghMtl, Lbuf, LbufMul2, BcMul, }
            public enum depth_test_func : int { Lequal, }
        }
        public class MK8D
        {
            public enum gsys_pass : int { no_setting, xlu_water, seal, }
            public enum gsys_priority_hint : int { object0, field_ground, field_wall, player, vr, effect, }
            public enum gsys_light_diffuse : int { diffuse, diffuse_course0, diffuse_ice0, diffuse_course3, diffuse_course2, }
            public enum gsys_env_obj_set : int { Turbo_area0, Turbo_area1, Turbo_area2, Turbo_area3, Turbo_area4, }
            public enum gsys_bake_group : int { group1, group2, none, group3, }
            public enum gsys_bake_uv_unite : int { none, set1, }
            public enum gsys_render_state_display_face : int { front, both, none, }
            public enum gsys_render_state_mode : int { opaque, mask, translucent, custom, }
            public enum gsys_depth_test_func : int { lequal, }
            public enum gsys_alpha_test_func : int { gequal, always, never, }
            public enum gsys_render_state_blend_mode : int { none, color, }
            public enum gsys_color_blend_rgb_op : int { add, max, }
            public enum gsys_color_blend_rgb_src_func : int { src_alpha, }
            public enum gsys_color_blend_rgb_dst_func : int { one_minus_src_alpha, one, }
            public enum gsys_color_blend_alpha_op : int { add, }
            public enum gsys_color_blend_alpha_src_func : int { one, }
            public enum gsys_color_blend_alpha_dst_func : int { zero, }
            public enum gsys_model_fx0 : int { no_setting, }
            public enum gsys_model_fx1 : int { no_setting, }
            public enum gsys_model_fx2 : int { no_setting, }
            public enum gsys_model_fx3 : int { no_setting, }
        }
        public class ARMS
        {
            public enum gsys_pass : int { no_setting, }
            public enum gsys_priority_hint : int { none, }
            public enum gsys_render_state_display_face : int { front, both, }
            public enum gsys_render_state_mode : int { opaque, mask, translucent, }
            public enum gsys_alpha_test_func : int { gequal, }
            public enum gsys_render_state_blend_mode : int { none, color, }
            public enum gsys_color_blend_rgb_src_func : int { src_alpha, }
            public enum gsys_color_blend_rgb_dst_func : int { one_minus_src_alpha, }
            public enum gsys_color_blend_rgb_op : int { add, }
            public enum gsys_color_blend_alpha_src_func : int { one, }
            public enum gsys_color_blend_alpha_dst_func : int { zero, }
            public enum gsys_color_blend_alpha_op : int { add, }
            public enum gsys_env_obj_set : int { ARMS_default, }
            public enum gsys_model_fx0 : int { no_setting, }
            public enum gsys_model_fx1 : int { no_setting, }
            public enum gsys_model_fx2 : int { no_setting, }
            public enum gsys_model_fx3 : int { no_setting, }
        }
        public class Splatoon2
        {
            public enum gsys_pass : int { no_setting, xlu_water, seal, }
            public enum gsys_env_obj_set : int { TPS, }
            public enum gsys_render_state_mode : int { mask, opaque, translucent, custom, }
            public enum gsys_render_state_display_face : int { both, front, none, }
            public enum gsys_depth_test_func : int { lequal, always, less, }
            public enum gsys_alpha_test_func : int { gequal, greater, }
            public enum gsys_render_state_blend_mode : int { none, color, }
            public enum gsys_color_blend_rgb_op : int { add, }
            public enum gsys_color_blend_rgb_src_func : int { src_alpha, dst_alpha, src_color, }
            public enum gsys_color_blend_rgb_dst_func : int { one_minus_src_alpha, one, zero, one_minus_src_color, }
            public enum gsys_color_blend_alpha_op : int { add, src_minus_dst, }
            public enum gsys_color_blend_alpha_src_func : int { one, }
            public enum gsys_color_blend_alpha_dst_func : int { zero, }
            public enum gsys_bake_normal_map : int { default0, }
            public enum gsys_bake_emission_map : int { default0, _e0, }
            public enum gsys_bake_group : int { group1, none, group2, group3, group4, }
            public enum gsys_bake_uv_unite : int { none, }
            public enum gsys_bake_option : int { none, option1, }
            public enum blitz_dynamic_alpha_fadeout : int { off, overlook, blend_vr, }
            public enum enable_aging_graffiti : int { off, }
            public enum aging_type : int { simple, }
            public enum enable_miiverse_filter : int { off, on, }
        }
        public class BOTW
        {
            public enum gsys_priority_hint : int { npc, player, field_ground, effect, vr, object0, field_wall, none, }
            public enum gsys_env_obj_set : int { Default, }
            public enum gsys_bake_group : int { group1, }
            public enum gsys_bake_uv_unite : int { none, }
            public enum gsys_bake_option : int { none, }
            public enum gsys_render_state_mode : int { opaque, mask, custom, translucent, }
            public enum gsys_render_state_display_face : int { front, both, back, }
            public enum gsys_render_state_blend_mode : int { none, color, }
            public enum gsys_depth_test_func : int { lequal, }
            public enum gsys_color_blend_rgb_src_func : int { src_alpha, }
            public enum gsys_color_blend_rgb_dst_func : int { one_minus_src_alpha, one, }
            public enum gsys_color_blend_rgb_op : int { add, }
            public enum gsys_color_blend_alpha_src_func : int { one, }
            public enum gsys_color_blend_alpha_dst_func : int { zero, }
            public enum gsys_color_blend_alpha_op : int { add, }
            public enum gsys_alpha_test_func : int { gequal, }
        }
    }
}
