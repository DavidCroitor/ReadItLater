import { useRouter } from 'next/router';
import { useState, useEffect } from 'react';
import useArticleDetails from '@/hooks/useArticleDetails';
import styles from '@/styles/articlePage.module.css';
import ReactMarkdown from 'react-markdown';


export default function ArticlePage() {
    const router = useRouter();
    const { id } = router.query; // Extract the article ID from the URL
    const { article, loading, error } = useArticleDetails(id); // Fetch articles using the custom hook

    if(loading) {
        return (
            <div className={styles.articlePageContainer}>
                <p>Loading article...</p>
            </div>
        );
    }

    if (error) {
        return (
            <div className={styles.articlePageContainer}>
                <p>Error loading article: {error}</p>
            </div>
        );
    }


    if (!article) {
        return (
            <div className={styles.articlePageContainer}>
                <p>Article not found</p>
            </div>
        );
    }

    return (
        <div className={styles.articlePageContainer}>
            <header className={styles.articleHeader}>
                <h2>{article.title}</h2>
                <div className={styles.indicators}>
                    {article.isRead && <span className={styles.readIndicator} title="Read">✓</span>}
                    {article.isFavourite && <span className={styles.favoriteIndicator} title="Favorite">⭐</span>}
                </div>
                <p className={styles.articleDate}>Saved on: {new Date(article.savedAt).toLocaleDateString("en-UK", {
                    year: "numeric",
                    month: "long",
                    day: "numeric",
                })}</p>
                <a className={styles.articleSource} href={article.url}>{article.url}</a>
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