shader_type spatial;

uniform sampler2DArray texture_albedo : hint_albedo;

void fragment() {
	vec4 albedo_tex = texture(texture_albedo, vec3(UV, UV2.x));
	ALBEDO = albedo_tex.rgb * COLOR.rgb;
}
