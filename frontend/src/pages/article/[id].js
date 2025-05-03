import { useRouter } from 'next/router';
import { useState, useEffect } from 'react';
import useArticles from '@/hooks/useArticles';
import styles from '@/styles/articlePage.module.css';
import ReactMarkdown from 'react-markdown';


export default function ArticlePage() {
    const router = useRouter();
    const { id } = router.query; // Extract the article ID from the URL
    const { articles, handleFavorite, handleRead } = useArticles(); // Fetch articles using the custom hook
    const [article, setArticle] = useState(null);

    useEffect(() => {
        if (id && articles) {
            const parsedId = parseInt(id, 10); // Parse the ID to an integer
            const foundArticle = articles.find(article => article.id === parsedId);
            setArticle(foundArticle);
        }
    }, [id, articles]);

    if (!article) {
        return (
            <div className={styles.articlePageContainer}>
                <p>Loading article...</p>
            </div>
        );
    }

    return (
        <div className={styles.articlePageContainer}>
            <header className={styles.articleHeader}>
                <h2>{article.title}</h2>
            </header>
            <div className={styles.articleContent}>
                <ReactMarkdown 
                >
                    {article.content}
                </ReactMarkdown>
                <a href={article.url} target="_blank" rel="noopener noreferrer">
                    Read Full Article
                </a>
            </div>
        </div>
    );
}