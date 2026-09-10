# FFmpeg bindings

ThumbGen binds to FFmpeg through two packages:

| Package | Version | Role |
| --- | --- | --- |
| `FFmpeg.AutoGen` (+ `.Abstractions`, `.Bindings.DynamicallyLoaded`) | 8.1.0 | Raw P/Invoke bindings, generated from the FFmpeg 8.1 headers |
| `FFmpeg4Sharp` (namespace `FFmpegSharp`) | 7.0.1 | Managed wrapper over the abstractions |

The native FFmpeg shared libraries you point `ffmpeg.RootPath` at must therefore be
**8.1.x**. The binding version tracks the FFmpeg version, and the major SO versions it
expects (`libavcodec.so.62`, `libavutil.so.60`, `libavformat.so.62`) only exist in
FFmpeg 8.x. CI installs 8.1.2.

## Why FFmpeg4Sharp stays on 7.0.1

7.0.1 is the latest release of FFmpeg4Sharp; there is no 8.x. It is compiled against
`FFmpeg.AutoGen.Abstractions` 7.0.0, so pairing it with the 8.1.0 abstractions is a
binary-compatibility question rather than a compile-time one.

Comparing the member references FFmpeg4Sharp emits against the members the 8.1.0
assembly actually declares, 7 of 486 are gone. All 7 are struct fields that FFmpeg 8
removed from its public headers:

- `AVFilter.formats`, `.formats_state`, `.nb_inputs`, `.nb_outputs`, `.priv_size`, `.flags_internal`
- `AVFrame.palette_has_changed`

Those fields are reached only through the `MediaFilter` properties and
`MediaFrame.PaletteHasChanged`. The CLR resolves field references lazily, when a method
is JIT-compiled, so a member that is never called never fails. ThumbGen uses only
`MediaDemuxer`, `MediaStream`, `MediaDecoder`, `MediaFrame` and `PixelConverter`, whose
members are all unchanged in 8.1.0.

**The constraint this puts on the code:** do not use FFmpegSharp's filter graph or
`MediaFrame.PaletteHasChanged`. Either one throws `MissingFieldException` at runtime, and
the build will not warn you. If ThumbGen needs filtering, FFmpeg4Sharp has to be replaced
or rebuilt against the 8.x abstractions first.

`ThumbGen.Magick` also hardcodes two `AVPixelFormat` values, 28 for `AV_PIX_FMT_BGRA` and
3 for `AV_PIX_FMT_BGR24`. Both are unchanged in FFmpeg 8.1, but they are worth
re-checking on the next major bump.

## Verifying a version bump

The test suite does not cover this, because a missing member only surfaces when the
affected method runs. Two checks are worth doing by hand after changing the binding
version:

1. Diff the member references FFmpeg4Sharp emits against the members the new
   `FFmpeg.AutoGen.Abstractions` declares, and confirm anything missing sits in a type
   ThumbGen does not touch.
2. Run an end-to-end extraction against the new native libraries and against the previous
   version, then compare. Frame dimensions, pixel format, PTS, timestamps and encoded
   output size should be identical. The 7.1.1 to 8.1.0 bump produced byte-identical
   results on `ThumbGen.Tests/bbb-h264-360p.mp4`.
