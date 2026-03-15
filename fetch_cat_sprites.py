import requests
from PIL import Image, ImageDraw
from io import BytesIO
from pathlib import Path

OUT = Path("cat_sprites")
OUT.mkdir(exist_ok=True)
SIZE = 256
urls = set()

# Fetch in batches of 10 until we have 50 unique URLs
while len(urls) < 50:
    data = requests.get("https://api.thecatapi.com/v1/images/search", params={"limit": 10}).json()
    for cat in data:
        urls.add(cat["url"])
        if len(urls) >= 50:
            break

for i, url in enumerate(urls):
    try:
        resp = requests.get(url, timeout=15)
        img = Image.open(BytesIO(resp.content)).convert("RGBA")

        # Center crop to square
        s = min(img.size)
        left = (img.width - s) // 2
        top = (img.height - s) // 2
        img = img.crop((left, top, left + s, top + s)).resize((SIZE, SIZE), Image.LANCZOS)

        # Apply circular mask
        mask = Image.new("L", (SIZE, SIZE), 0)
        ImageDraw.Draw(mask).ellipse((0, 0, SIZE, SIZE), fill=255)
        img.putalpha(mask)

        img.save(OUT / f"cat_{i:02d}.png")
        print(f"[{i+1}/50] saved cat_{i:02d}.png")
    except Exception as e:
        print(f"[{i+1}/50] failed: {e}")

print(f"\nDone! {len(list(OUT.glob('*.png')))} sprites in '{OUT}/'")
