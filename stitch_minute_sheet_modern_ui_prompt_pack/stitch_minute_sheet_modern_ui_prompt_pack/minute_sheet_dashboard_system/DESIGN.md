---
name: Minute Sheet Dashboard System
colors:
  surface: '#f9f9ff'
  surface-dim: '#d3daea'
  surface-bright: '#f9f9ff'
  surface-container-lowest: '#ffffff'
  surface-container-low: '#f0f3ff'
  surface-container: '#e7eefe'
  surface-container-high: '#e2e8f8'
  surface-container-highest: '#dce2f3'
  on-surface: '#151c27'
  on-surface-variant: '#544438'
  inverse-surface: '#2a313d'
  inverse-on-surface: '#ebf1ff'
  outline: '#867366'
  outline-variant: '#d9c2b3'
  surface-tint: '#904d00'
  primary: '#904d00'
  on-primary: '#ffffff'
  primary-container: '#f2994a'
  on-primary-container: '#663500'
  inverse-primary: '#ffb77d'
  secondary: '#555c85'
  on-secondary: '#ffffff'
  secondary-container: '#c8cfff'
  on-secondary-container: '#505780'
  tertiary: '#574bc7'
  on-tertiary: '#ffffff'
  tertiary-container: '#aaa3ff'
  on-tertiary-container: '#3929a9'
  error: '#ba1a1a'
  on-error: '#ffffff'
  error-container: '#ffdad6'
  on-error-container: '#93000a'
  primary-fixed: '#ffdcc3'
  primary-fixed-dim: '#ffb77d'
  on-primary-fixed: '#2f1500'
  on-primary-fixed-variant: '#6e3900'
  secondary-fixed: '#dee1ff'
  secondary-fixed-dim: '#bdc4f3'
  on-secondary-fixed: '#11183e'
  on-secondary-fixed-variant: '#3d446c'
  tertiary-fixed: '#e4dfff'
  tertiary-fixed-dim: '#c5c0ff'
  on-tertiary-fixed: '#150067'
  on-tertiary-fixed-variant: '#3f30ae'
  background: '#f9f9ff'
  on-background: '#151c27'
  surface-variant: '#dce2f3'
typography:
  headline-xl:
    fontFamily: Inter
    fontSize: 32px
    fontWeight: '600'
    lineHeight: 40px
    letterSpacing: -0.02em
  headline-lg:
    fontFamily: Inter
    fontSize: 24px
    fontWeight: '600'
    lineHeight: 32px
    letterSpacing: -0.01em
  headline-md:
    fontFamily: Inter
    fontSize: 20px
    fontWeight: '600'
    lineHeight: 28px
  body-lg:
    fontFamily: Inter
    fontSize: 16px
    fontWeight: '400'
    lineHeight: 24px
  body-md:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '400'
    lineHeight: 20px
  body-sm:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '400'
    lineHeight: 16px
  label-md:
    fontFamily: Inter
    fontSize: 14px
    fontWeight: '600'
    lineHeight: 20px
  label-sm:
    fontFamily: Inter
    fontSize: 12px
    fontWeight: '600'
    lineHeight: 16px
rounded:
  sm: 0.25rem
  DEFAULT: 0.5rem
  md: 0.75rem
  lg: 1rem
  xl: 1.5rem
  full: 9999px
spacing:
  sidebar-width: 260px
  topbar-height: 64px
  container-padding: 24px
  gutter: 16px
  stack-sm: 8px
  stack-md: 16px
  stack-lg: 24px
---

## Brand & Style
The design system is engineered for high-stakes enterprise governance and administrative efficiency. It targets executive leadership and administrative officers who require a high-density, high-clarity environment for reviewing and approving official documentation.

The aesthetic follows a **Corporate / Modern** movement, emphasizing reliability through a structured sidebar navigation and a clear content hierarchy. The interface prioritizes functional clarity over decorative elements, using a white "surface-on-canvas" model to create distinct work zones. The emotional response is one of precision, institutional trust, and streamlined workflow, avoiding visual fatigue through a balanced use of white space and a refined, professional color palette.

## Colors
This design system utilizes a structured hierarchical color palette:
- **Primary Interface Layers:** The sidebar uses `navy-800` to ground the navigation, while the top bar uses `navy-900` for the highest level of structural authority.
- **Action & Accent:** `orange-500` is reserved for primary calls to action (buttons, active states). `orange-50` provides a soft highlight for hovered or selected rows and subtle section backgrounds.
- **Surface Strategy:** Content sits on `surface` (#FFFFFF) cards, which are visually separated from the `canvas` (#F4F6FB) background. 
- **Semantic Statuses:** Five distinct color pairings are used for document lifecycle management (Approved, Pending, Returned, Draft, Marked). Each status uses a high-contrast text color against a low-opacity background for maximum legibility in high-density tables.

## Typography
The system uses **Inter** exclusively to ensure a systematic, neutral, and highly readable experience across technical data. 
- **Hierarchy:** Use `headline-xl` only for dashboard titles. Secondary page headers utilize `headline-lg`.
- **Data Display:** Tables and form fields primarily use `body-md` for optimal information density.
- **Weighting:** Headings are strictly set to 600 (Semi-bold) to provide clear structural anchoring. Body text is 400 (Regular). 
- **Captions:** Use `body-sm` for metadata and timestamps to maintain a clean visual hierarchy.

## Layout & Spacing
The layout follows a **Fixed Sidebar + Fluid Content** model. 
- **Grid:** A 12-column fluid grid system is used within the main content area.
- **Margins:** Page-level margins are fixed at 24px (`container-padding`) to ensure content doesn't bleed to the screen edges.
- **Spacing Rhythm:** Use an 8px base unit. 16px is the standard gutter for grid items.
- **Responsive Behavior:** 
  - **Desktop (>1024px):** Permanent sidebar, 24px padding.
  - **Tablet (768px - 1023px):** Sidebar collapses to icons only, padding reduces to 16px.
  - **Mobile (<767px):** Sidebar becomes a hidden drawer, top bar remains sticky, and table views transition to card-based layouts.

## Elevation & Depth
Depth is conveyed through **Tonal Layers** supplemented by **Ambient Shadows**. 
- **Level 0 (Canvas):** The base background layer (#F4F6FB).
- **Level 1 (Cards/Surface):** The primary container for data. These use a 1px border (#E6E9F2) and a very soft shadow (0px 2px 4px rgba(27, 31, 59, 0.04)).
- **Level 2 (Popovers/Modals):** Elements that float above the surface. These use a more pronounced shadow (0px 10px 15px -3px rgba(27, 31, 59, 0.1)) to indicate focus and interactivity.
- **Transitions:** Hover states on interactive cards should slightly deepen the shadow and lighten the border color to #D1D5DB.

## Shapes
The design system employs a **Rounded** shape language to soften the corporate environment and improve visual scanning.
- **Containers:** All cards, tables, and modal containers use a 12px (`rounded-lg`) corner radius.
- **Interactive Elements:** Primary buttons and input fields use an 8px (`rounded-md`) radius.
- **Status Badges:** These are strictly **Pill-shaped** (full radius) to distinguish them from other interactive UI components and ensure they are immediately recognizable as status indicators.

## Components
- **Buttons:** Primary buttons use `orange-500` with white text. Ghost buttons use `text-secondary` with no background. All buttons have an 8px radius and horizontal padding of 16px.
- **Status Badges:** Use the semantic color palette defined in the Colors section. Text is semi-bold, 12px, and centered within a 24px high pill container.
- **Tables:** Headers are sticky, using `canvas` background and `label-sm` typography. Row separators are 1px `border`. High-density rows should have a hover state of `orange-50`.
- **Input Fields:** 1px `border` (#E6E9F2), 8px radius, with 12px internal horizontal padding. On focus, the border changes to `orange-500` with a 2px outer glow.
- **Sidebar Nav:** Active items use a left-aligned 4px vertical bar in `orange-500` and a subtle background highlight.
- **Minute Sheets:** Specific component for document viewing; a centered 8.5x11 aspect ratio surface with high-contrast text and a vertical "Timeline" or "Trail" on the right side for approval history.