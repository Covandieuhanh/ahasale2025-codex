Static region hero images for `/bat-dong-san`.

Province alias and image-generation standard lives at:
- `assets/img/bds-region/province-aliases.yaml`
- Includes canonical 63-province slug aliases, style, overlay, naming, and processing rules.

Current resolver supports multiple extensions and auto picks the first existing file:
1. `.jpg`
2. `.jpeg`
3. `.webp`
4. `.png`
5. `.svg`

Naming rule:
- `<province-slug>.<ext>` (example: `tp-ho-chi-minh.jpg`, `ha-noi.jpg`)

Fallback files:
- `default.jpg`
- `default.svg` (secondary fallback)

Deploy:
1. Upload the whole `assets/img/bds-region/` folder.
2. Keep filenames exactly by province slug.
3. Hard refresh after deploy.

The module currently includes alias images for all province slugs in old 63-province mapping,
so region cards will not break on host when deploying this folder.
