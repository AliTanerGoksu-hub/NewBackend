from PIL import Image, ImageDraw, ImageFont

# 1024x1024 PNG icon oluştur
size = 1024
img = Image.new('RGB', (size, size), color='#6C5CE7')

# BSM yazısını ekle
draw = ImageDraw.Draw(img)

# Basit dikdörtgen çiz (logo yerine)
draw.rectangle([200, 400, 824, 624], fill='white')
draw.rectangle([220, 420, 804, 604], fill='#6C5CE7')

# PNG olarak kaydet
img.save('appicon.png')
print("✓ appicon.png oluşturuldu")
