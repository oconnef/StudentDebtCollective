# Student Debt Collective — Redesign Notes

## What changed
- **index.html** — complete rewrite, Variant A of A/B test ("Congress isn't coming")
- **index-b.html** — new file, Variant B ("One of us gets free every month")
- **faq.html** — new page with full FAQ copy
- **recipients.html** — new page with recipient card template
- **about.html** — rewritten with Tom's existing story, cleaned up
- **css/custom.css** — new custom theme (layered on Bootstrap 5)
- Old styles.css is preserved but no longer used by new pages

## To-do before launch

### 1. Fix SSL certificate (TODAY)
The cert at www.studentdebtcollective.com has a hostname mismatch.
Fix through your hosting provider or domain registrar.

### 2. Update member count
In both index.html and index-b.html, find:
```js
var MEMBER_COUNT = 23;
```
Update this number manually for now. Wire to a real API/database later.

### 3. Set up GA4
Your current tag is Universal Analytics (UA-217164909-1) which Google
sunset in 2023. Create a GA4 property, get a measurement ID (G-XXXXXXX),
and replace the UA ID in all pages.

### 4. A/B test — how it works
- index.html randomly assigns visitors to variant A or B (50/50)
- Assignment is stored in a cookie for 30 days (same visitor = same variant)
- Variant B visitors are immediately redirected to index-b.html
- Both variants fire a GA event: `ab_variant` with `{variant: 'a'}` or `{variant: 'b'}`
- Signups fire: `signup_intent` with the variant label

**How to read results in GA4:**
Explore > Funnel Exploration, filter by `ab_variant` event parameter.
Or use the Events report and compare signup_intent counts per variant.

### 5. Add real recipient cards
In recipients.html, there's a commented-out example card block.
When your first drawing happens:
1. Get written consent from the recipient
2. Uncomment and fill in the card
3. Remove the placeholder "coming soon" block

### 6. Talk to a CPA
Before your first payoff, get legal clarity on:
- 501(c)(3) nonprofit status
- Gift tax implications for recipients
- Payment structure (pay lender directly = best)

## File structure
```
/
├── index.html          ← Variant A (A/B controller lives here)
├── index-b.html        ← Variant B
├── faq.html            ← FAQ page (new)
├── recipients.html     ← Who we've helped (new)
├── about.html          ← Rewritten
├── contact.html        ← Unchanged
├── TandC.html          ← Unchanged
├── privacypolicy.html  ← Unchanged
├── css/
│   ├── custom.css      ← NEW — all custom styles
│   └── styles.css      ← Old Bootstrap bundle (still present, not used)
└── assets/             ← Unchanged
```
