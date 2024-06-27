CREATE INDEX idx_sales_managers_gin ON sales_managers USING GIN (languages, products, customer_ratings);

CREATE INDEX idx_slots_sales_manager_id_start_date_end_date ON slots (sales_manager_id, tsrange(start_date, end_date '[]'));