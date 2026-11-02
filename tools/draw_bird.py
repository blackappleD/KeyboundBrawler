# Draws the typing bird, Steam Bongo Cat style: a chubby white bird with glasses peeking over a ledge,
# wings up while resting, one wing bonking the ledge per key. Three frames: idle, left, right.
# Usage: python -I tools/draw_bird.py images [preview.png]
import sys
from PIL import Image, ImageDraw

S = 4                      # supersampling
W, H = 300, 290
INK = (84, 56, 44, 255)
WHITE = (255, 255, 255, 255)
SHADE = (244, 236, 228, 255)
PINK = (255, 178, 186, 255)
BEAK = (247, 168, 92, 255)
LEDGE = (255, 241, 222, 255)
LW = 6                     # outline width before supersampling
LEDGE_Y = 214              # top of the ledge the bird peeks over


def s(v): return [round(c * S) for c in v]


def layer():
    img = Image.new("RGBA", (W * S, H * S), (0, 0, 0, 0))
    return img, ImageDraw.Draw(img)


def line(d, pts, w=LW):
    d.line(s(pts), fill=INK, width=w * S, joint="curve")
    for x, y in (pts[:2], pts[-2:]):   # round caps
        r = w / 2
        d.ellipse(s((x - r, y - r, x + r, y + r)), fill=INK)


def body(d):
    d.ellipse(s((40, 62, 260, 300)), fill=WHITE, outline=INK, width=LW * S)
    d.ellipse(s((92, 170, 208, 240)), fill=SHADE)          # soft belly shade
    # Head tuft: three soft curls.
    for x, h in ((138, 14), (152, 22), (166, 13)):
        d.arc(s((x - 8, 63 - h, x + 8, 63 + h)), 200, 340, fill=INK, width=LW * S)


def face(d, happy):
    for cx in (116, 184):
        d.ellipse(s((cx - 26, 104, cx + 26, 156)), outline=INK, width=5 * S)   # glasses
        if happy:
            d.arc(s((cx - 11, 124, cx + 11, 142)), 200, 340, fill=INK, width=5 * S)  # ^ ^
        else:
            d.ellipse(s((cx - 8, 122, cx + 8, 140)), fill=INK)
            d.ellipse(s((cx - 4, 124, cx + 1, 129)), fill=WHITE)
    d.arc(s((138, 118, 162, 134)), 200, 340, fill=INK, width=5 * S)          # bridge
    for cx in (84, 216):
        d.ellipse(s((cx - 15, 150, cx + 15, 166)), fill=PINK)                 # blush
    if happy:   # beak open mid-bonk
        d.polygon(s((140, 156, 160, 156, 150, 166)), fill=BEAK, outline=INK, width=3 * S)
        d.polygon(s((143, 168, 157, 168, 150, 176)), fill=BEAK, outline=INK, width=3 * S)
    else:
        d.polygon(s((140, 157, 160, 157, 150, 171)), fill=BEAK, outline=INK, width=3 * S)


def ledge(d):
    d.rounded_rectangle(s((-20, LEDGE_Y, W + 20, H + 30)), radius=18 * S, fill=LEDGE, outline=INK, width=LW * S)


def wing_up(d, cx):
    # Raised wing tip, round like a paw, with pink pads on its face.
    d.ellipse(s((cx - 28, LEDGE_Y - 62, cx + 28, LEDGE_Y - 8)), fill=WHITE, outline=INK, width=LW * S)
    d.ellipse(s((cx - 11, LEDGE_Y - 36, cx + 11, LEDGE_Y - 20)), fill=PINK)
    for dx, dy in ((-16, -46), (0, -52), (16, -46)):
        d.ellipse(s((cx + dx - 5, LEDGE_Y + dy - 5, cx + dx + 5, LEDGE_Y + dy + 5)), fill=PINK)


def wing_down(d, cx, side):
    # Flattened wing squashed on the ledge, with little bonk lines.
    d.ellipse(s((cx - 36, LEDGE_Y - 18, cx + 36, LEDGE_Y + 20)), fill=WHITE, outline=INK, width=LW * S)
    for dx in (-14, 2, 18):
        d.arc(s((cx + dx - 7, LEDGE_Y - 2, cx + dx + 7, LEDGE_Y + 14)), 200, 340, fill=INK, width=3 * S)
    out = -1 if side == "left" else 1
    for ang in (-1, 0, 1):
        x0 = cx + out * (44 + abs(ang) * 2)
        y0 = LEDGE_Y - 4 + ang * 16
        line(d, (x0, y0, x0 + out * 14, y0 + ang * 6), 4)


def frame(down):
    img, d = layer()
    body(d)
    face(d, happy=down is not None)
    ledge(d)
    for side, cx in (("left", 62), ("right", 238)):
        if side == down:
            wing_down(d, 84 if side == "left" else 216, side)
        else:
            wing_up(d, cx)
    return img.resize((W, H), Image.LANCZOS)


out = sys.argv[1]
frames = {"idle": frame(None), "left": frame("left"), "right": frame("right")}
for name, f in frames.items():
    f.save(f"{out}/bird_{name}.png")
if len(sys.argv) > 2:
    preview = Image.new("RGBA", (W * 3 + 40, H + 20), (253, 247, 234, 255))
    for i, f in enumerate(frames.values()):
        preview.alpha_composite(f, (10 + i * (W + 10), 10))
    preview.save(sys.argv[2])
