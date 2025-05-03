import re
import sys
import time
import trafilatura
from  urllib.parse import urlparse
import requests
import argparse
import io

sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8', errors='replace')
sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding='utf-8', errors='replace')

class ArticleScraper:
    def __init__(self, user_agent=None,
                request_delay=1,
                request_timeout=15):
        
        self.user_agent = user_agent if user_agent else 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3'
        self.request_delay = request_delay
        self.request_timeout = request_timeout

        self.headers = {
            'User-Agent': self.user_agent,
            'Accept-Language': 'en-US,en;q=0.9',
            'Accept-Encoding': 'gzip, deflate, br',
            'Connection': 'keep-alive'
        }

    def get_raw_html(self, url):
        """
        Fetch the raw HTML content of a webpage.
        """
        try:
            response = requests.get(url, headers=self.headers, timeout=self.request_timeout)
            response.raise_for_status()  # Raise an error for bad responses
            return response.text
        except requests.RequestException as e:
            print(f"Error fetching {url}: {e}")
            return None
    
    def get_title(self, url):
        raw_html = self.get_raw_html(url)
        if raw_html:
            title = trafilatura.extract_metadata(raw_html).title
            if title:
                return title
            else:
                print(f"Error extracting title from {url}")
                return None
        return None

    def get_markdown(self, url):
        """
        Fetch the raw HTML content of a webpage and convert it to markdown.
        """
        raw_html = self.get_raw_html(url)
        if raw_html:
            # Use trafilatura to extract the text and convert it to markdown
            article = trafilatura.extract(raw_html, url=url, output_format='markdown')
            if article:
                return self.clean_text(article)
            else:
                print(f"Error extracting text from {url}")
                return None
        return None

    def clean_text(self, text):
        """
        Clean the extracted text by removing unnecessary elements.
        """
        cleaned_text = re.sub(r'\[[^\]]+\]', '', text)  # Remove links in brackets

        unwanted_sections = [
            'advertisement',
            'sponsored',
            'related articles',
            'disclaimer',
            'references',
            'source',
            'external links',
            'advertisement',
            'sponsored',
            'related articles',
            'related posts',
            'you may also like',
            'popular posts',
            'latest posts',
            'see also',
            'frequently asked questions',
            'faq',
            'additional resources'
        ]
        unwanted_sections.sort(key=len, reverse=True)  # Sort by length to avoid partial matches
        for section in unwanted_sections:
            pattern = re.compile(
            r'^#{1,6}\s*' + re.escape(section) + r'\s*\n?[\s\S]*?(?=\n^#{1,6}\s|\Z)',
            flags=re.IGNORECASE | re.MULTILINE
        )
            cleaned_text = pattern.sub('', cleaned_text)

       

        return cleaned_text.strip()
    

if __name__ == "__main__":
    # --- Argument Parsing Setup ---
    parser = argparse.ArgumentParser(description="Scrape and clean article content from a URL.")
    
    # Add the URL argument (positional, required)
    parser.add_argument("url", help="The full URL of the article to scrape")

    # Add optional arguments (example)
    parser.add_argument("-t", "--title", action="store_true", help="Fetch the title of the article")

    # Parse the arguments provided by the user
    args = parser.parse_args()
    # --- End Argument Parsing Setup ---


    # Now use the parsed arguments
    # You can access the URL via args.url
    # If you added optional args like timeout/delay, access them like args.timeout, args.delay

    # Example using parsed URL and default timeout/delay from the class definition
    scraper = ArticleScraper() # You could pass args.timeout, args.delay here if you defined them above

    target_url = args.url # Get the URL from the parsed arguments
    title_flag = args.title # Check if the title flag was provided


    print(f"Fetching and processing: {target_url}")
    markdown_content = scraper.get_markdown(target_url)
    title = scraper.get_title(target_url)
    if title_flag:
        print("\n--- Title ---")
        print(title)
        print("\n--- End of Title ---")
    elif markdown_content:
        print("\n--- Cleaned Markdown Content ---")
        print(markdown_content)
        print("\n--- End of Content ---")
    else:
        print("Failed to fetch or convert the article.")
        sys.exit(1)