// @ts-check

// This runs in Node.js - Don't use client-side code here (browser APIs, JSX...)

/**
 * Creating a sidebar enables you to:
 - create an ordered group of docs
 - render a sidebar for each doc of that group
 - provide next/previous navigation

 The sidebars can be generated from the filesystem, or explicitly defined here.

 Create as many sidebars as you want.

 @type {import('@docusaurus/plugin-content-docs').SidebarsConfig}
 */
const sidebars = {
  tutorialSidebar: [
    'intro',
    {
      type: 'category',
      label: 'Getting Started',
      items: [
        'getting-started/installation',
        'getting-started/quick-start',
        'getting-started/why-sguard',
      ],
    },
    {
      type: 'category',
      label: 'Core Concepts',
      items: [
        'core-concepts/guard-methods',
        'core-concepts/callbacks',
        'core-concepts/custom-exceptions',
        'core-concepts/expression-caching',
      ],
    },
    {
      type: 'category',
      label: 'Guides',
      items: [
        'guides/null-empty-checks',
        'guides/comparison-guards',
        'guides/collection-validation',
        'guides/string-comparisons',
        'guides/real-world-examples',
      ],
    },
    {
      type: 'category',
      label: 'Advanced',
      items: [
        'advanced/performance',
        'advanced/best-practices',
      ],
    },
    {
      type: 'category',
      label: 'API Reference',
      items: [
        'api/throwif',
        'api/is',
        'api/callbacks',
      ],
    },
    {
      type: 'category',
      label: 'Community',
      items: [
        'community/contributing',
        'community/code-of-conduct',
        'community/changelog',
      ],
    },
  ],
};

export default sidebars;
