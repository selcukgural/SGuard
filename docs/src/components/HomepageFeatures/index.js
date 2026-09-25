import clsx from 'clsx';
import Heading from '@theme/Heading';
import styles from './styles.module.css';

const FeatureList = [
  {
    title: 'Expressive and Robust',
    description: (
      <>
        Two complementary APIs: <code>ThrowIf.*</code> for fail-fast validation 
        and <code>Is.*</code> for boolean checks. Choose the style that fits your code.
      </>
    ),
  },
  {
    title: 'Precise Diagnostics',
    description: (
      <>
        Uses <code>CallerArgumentExpression</code> to automatically generate 
        clear, helpful error messages that point to the exact argument that failed.
      </>
    ),
  },
  {
    title: 'Comprehensive Guards',
    description: (
      <>
        Null/empty checks, comparison guards, collection and span validation,
        email validation, custom exceptions, callbacks, and culture-aware string comparisons.
      </>
    ),
  },
  {
    title: 'High Performance',
    description: (
      <>
        Selector expressions are compiled once and cached in a thread-safe cache.
        Benchmarks available for all guard methods.
      </>
    ),
  },
  {
    title: 'Modern .NET',
    description: (
      <>
        Multi-targeting support for .NET 8, 9, and 10. 
        Follows modern C# best practices and conventions.
      </>
    ),
  },
  {
    title: 'Easy to Extend',
    description: (
      <>
        Custom exception support, unified callback model, and built-in exceptions that derive from
        <code>ArgumentException</code>. 
        Integrate seamlessly with your domain logic.
      </>
    ),
  },
];

function Feature({title, description}) {
  return (
    <div className={clsx('col col--4')}>
      <div className="text--center padding-horiz--md">
        <Heading as="h3">{title}</Heading>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures() {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
